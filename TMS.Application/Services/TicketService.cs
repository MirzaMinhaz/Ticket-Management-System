using AutoMapper;
using TMS.Application.DTOs.Ticket;
using TMS.Application.Exceptions;
using TMS.Application.Interfaces;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Repositories;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TicketService(
            ITicketRepository ticketRepository,
            ISeatRepository seatRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _seatRepository = seatRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ── CRUD ────────────────────────────────────────────────────────────

        public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
        {
            try
            {
                var tickets = await _ticketRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<TicketDto>>(tickets);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to fetch tickets.", ex);
            }
        }

        public async Task<TicketDto?> GetTicketByIdAsync(int id)
        {
            try
            {
                var ticket = await _ticketRepository.GetByIdAsync(id);
                return ticket == null ? null : _mapper.Map<TicketDto>(ticket);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to fetch ticket with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(int userId)
        {
            try
            {
                var tickets = await _ticketRepository.GetByUserIdAsync(userId);
                return _mapper.Map<IEnumerable<TicketDto>>(tickets);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to fetch tickets for user {userId}.", ex);
            }
        }

        /// <summary>
        /// Creates a ticket with an atomic seat-conflict check. Runs inside a
        /// database transaction: re-reads the currently-booked seats for this
        /// trip immediately before insert (closing the race window that the
        /// SignalR soft-locks alone cannot close — those only coordinate
        /// well-behaved clients, not the final write), and rolls back with a
        /// SeatConflictException if anything requested is already taken.
        ///
        /// This is the layer that guarantees correctness during a rush: many
        /// concurrent agents can all pass the soft-lock UI stage, but only one
        /// of them will win the DB race for a given seat — everyone else gets
        /// a clean 409 instead of a silent double-booking.
        /// </summary>
        public async Task<TicketDto> CreateTicketAsync(CreateTicketDto dto, int? userId)
        {
            if (dto.TripId <= 0)
                throw new ArgumentException("A valid TripId is required to create a ticket.");

            var requestedSeats = SplitSeats(dto.SeatNumber);
            if (requestedSeats.Count == 0)
                throw new ArgumentException("At least one seat must be selected.");

            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var alreadyBooked = await _ticketRepository.GetActiveBookedSeatNumbersAsync(dto.TripId);
                var conflicts = requestedSeats.Where(s => alreadyBooked.Contains(s)).ToList();
                if (conflicts.Count > 0)
                {
                    await transaction.RollbackAsync();
                    throw new SeatConflictException(conflicts);
                }

                string ticketCode = await GenerateUniqueCode("TKT-", t => t.TicketCode);
                string seatCode = await GenerateUniqueCode("SEA-", t => t.SeatCode);

                var ticket = new Ticket
                {
                    UserId = userId ?? 0,
                    TripId = dto.TripId,
                    PassengerName = dto.PassengerName,
                    PassengerContact = dto.PassengerContact,
                    SeatNumber = dto.SeatNumber,
                    SeatCode = dto.SeatCode ?? seatCode,
                    FarePaid = dto.FarePaid,
                    BookingDateTime = dto.BookingDateTime,
                    BookingCounterId = dto.BookingCounterId,
                    DepartureCounterId = dto.DepartureCounterId,
                    ArrivalCounterId = dto.ArrivalCounterId,
                    TicketCode = ticketCode,
                    Status = "Booked",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = "System"
                };

                await _ticketRepository.AddAsync(ticket);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return _mapper.Map<TicketDto>(ticket);
            }
            catch (SeatConflictException)
            {
                throw; // let the controller map this to 409 — don't wrap it
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("Failed to create ticket.", ex);
            }
        }

        /// <summary>
        /// Same atomic-conflict pattern as CreateTicketAsync, but excludes the
        /// ticket being edited from the conflict check — so an agent editing a
        /// ticket doesn't get falsely blocked by their own previously-held seats.
        /// </summary>
        public async Task<TicketDto?> UpdateTicketAsync(UpdateTicketDto dto)
        {
            var requestedSeats = SplitSeats(dto.SeatNumber);

            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var ticket = await _ticketRepository.GetByIdAsync(dto.Id);
                if (ticket == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                if (requestedSeats.Count > 0)
                {
                    var alreadyBooked = await _ticketRepository.GetActiveBookedSeatNumbersAsync(dto.TripId, excludeTicketId: dto.Id);
                    var conflicts = requestedSeats.Where(s => alreadyBooked.Contains(s)).ToList();
                    if (conflicts.Count > 0)
                    {
                        await transaction.RollbackAsync();
                        throw new SeatConflictException(conflicts);
                    }
                }

                ticket.TripId = dto.TripId;
                ticket.PassengerName = dto.PassengerName;
                ticket.PassengerContact = dto.PassengerContact;
                ticket.SeatNumber = dto.SeatNumber;
                ticket.SeatCode = !string.IsNullOrEmpty(dto.SeatCode) ? dto.SeatCode : ticket.SeatCode;
                ticket.FarePaid = dto.FarePaid;
                ticket.BookingDateTime = dto.BookingDateTime;
                ticket.BookingCounterId = dto.BookingCounterId;
                ticket.DepartureCounterId = dto.DepartureCounterId;
                ticket.ArrivalCounterId = dto.ArrivalCounterId;
                ticket.LastModifiedAt = DateTime.UtcNow;
                ticket.LastModifiedBy = "System";

                _ticketRepository.Update(ticket);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return _mapper.Map<TicketDto>(ticket);
            }
            catch (SeatConflictException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException($"Failed to update ticket with ID {dto.Id}.", ex);
            }
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            try
            {
                var ticket = await _ticketRepository.GetByIdAsync(id);
                if (ticket == null) return false;

                await _ticketRepository.DeleteAsync(ticket);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to delete ticket with ID {id}.", ex);
            }
        }

        public async Task<TicketDto?> CancelTicketAsync(CancelTicketDto dto)
        {
            try
            {
                var ticket = await _ticketRepository.GetByIdAsync(dto.Id);
                if (ticket == null) return null;
                if (ticket.Status == "Cancelled") return _mapper.Map<TicketDto>(ticket);

                ticket.Status = "Cancelled";
                ticket.LastModifiedAt = DateTime.UtcNow;
                ticket.LastModifiedBy = "System";

                _ticketRepository.Update(ticket);
                await _unitOfWork.CompleteAsync();
                return _mapper.Map<TicketDto>(ticket);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to cancel ticket with ID {dto.Id}.", ex);
            }
        }

        // ── Legacy seat helpers ──────────────────────────────────────────────

        public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int scheduleId)
        {
            try
            {
                var seats = await _seatRepository.GetSeatsByScheduleAsync(scheduleId);
                return seats.Where(s => !s.IsBooked);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to fetch available seats.", ex);
            }
        }

        public async Task<bool> BookSeatAsync(int seatId, string userId)
        {
            try
            {
                var seat = await _seatRepository.GetSeatByIdAsync(seatId);
                if (seat == null || seat.IsBooked) return false;
                seat.IsBooked = true;
                seat.LastModifiedBy = userId;
                seat.LastModifiedAt = DateTime.UtcNow;
                await _seatRepository.UpdateSeatAsync(seat);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to book seat with ID {seatId}.", ex);
            }
        }

        public async Task<bool> CancelSeatAsync(int seatId, string userId)
        {
            try
            {
                var seat = await _seatRepository.GetSeatByIdAsync(seatId);
                if (seat == null || !seat.IsBooked) return false;
                seat.IsBooked = false;
                seat.LastModifiedBy = userId;
                seat.LastModifiedAt = DateTime.UtcNow;
                await _seatRepository.UpdateSeatAsync(seat);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to cancel seat with ID {seatId}.", ex);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static List<string> SplitSeats(string? seatNumberCsv)
        {
            if (string.IsNullOrWhiteSpace(seatNumberCsv)) return new List<string>();
            return seatNumberCsv
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        private async Task<string> GenerateUniqueCode(string prefix, Func<Ticket, string?> selector)
        {
            try
            {
                var all = await _ticketRepository.GetAllAsync();
                var last = all
                    .Select(selector)
                    .Where(c => !string.IsNullOrEmpty(c) && c!.StartsWith(prefix))
                    .OrderByDescending(c => c)
                    .FirstOrDefault();

                int next = 1;
                if (!string.IsNullOrEmpty(last))
                {
                    var numPart = last!.Substring(last.LastIndexOf('-') + 1);
                    if (int.TryParse(numPart, out int n)) next = n + 1;
                }
                return $"{prefix}{next:D3}";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to generate unique code.", ex);
            }
        }
    }
}