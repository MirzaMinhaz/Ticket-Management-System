// TMS.Application.Services/TicketService.cs
using AutoMapper;
using TMS.Application.DTOs.Ticket;
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
            var tickets = await _ticketRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TicketDto>>(tickets);
        }

        public async Task<TicketDto?> GetTicketByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            return ticket == null ? null : _mapper.Map<TicketDto>(ticket);
        }

        public async Task<TicketDto> CreateTicketAsync(CreateTicketDto dto)
        {
            // TripId must be resolved BEFORE this call (done by TripService on the frontend trigger)
            if (dto.TripId <= 0)
                throw new ArgumentException("A valid TripId is required to create a ticket.");

            string ticketCode = await GenerateUniqueCode("TKT-", t => t.TicketCode);
            string seatCode = await GenerateUniqueCode("SEA-", t => t.SeatCode);

            var ticket = new Ticket
            {
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
            return _mapper.Map<TicketDto>(ticket);
        }

        public async Task<TicketDto?> UpdateTicketAsync(UpdateTicketDto dto)
        {
            var ticket = await _ticketRepository.GetByIdAsync(dto.Id);
            if (ticket == null) return null;

            ticket.TripId = dto.TripId;
            ticket.PassengerName = dto.PassengerName;
            ticket.PassengerContact = dto.PassengerContact;
            ticket.SeatNumber = dto.SeatNumber;
            ticket.SeatCode = dto.SeatCode;
            ticket.FarePaid = dto.FarePaid;
            ticket.BookingDateTime = dto.BookingDateTime;
            ticket.BookingCounterId = dto.BookingCounterId;
            ticket.DepartureCounterId = dto.DepartureCounterId;
            ticket.ArrivalCounterId = dto.ArrivalCounterId;
            ticket.LastModifiedAt = DateTime.UtcNow;
            ticket.LastModifiedBy = "System";

            _ticketRepository.Update(ticket);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<TicketDto>(ticket);
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null) return false;

            await _ticketRepository.DeleteAsync(ticket);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        /// <summary>
        /// Soft-cancel: sets Status = "Cancelled", does NOT delete the row.
        /// The seat is freed automatically because GetBookedSeats filters by Status = "Booked".
        /// </summary>
        public async Task<TicketDto?> CancelTicketAsync(CancelTicketDto dto)
        {
            var ticket = await _ticketRepository.GetByIdAsync(dto.Id);
            if (ticket == null) return null;
            if (ticket.Status == "Cancelled") return _mapper.Map<TicketDto>(ticket); // idempotent

            ticket.Status = "Cancelled";
            ticket.LastModifiedAt = DateTime.UtcNow;
            ticket.LastModifiedBy = "System";
            // Optionally store dto.Reason in a Notes/Reason column if you add one

            _ticketRepository.Update(ticket);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<TicketDto>(ticket);
        }

        // ── Legacy seat helpers ──────────────────────────────────────────────

        public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int scheduleId)
        {
            var seats = await _seatRepository.GetSeatsByScheduleAsync(scheduleId);
            return seats.Where(s => !s.IsBooked);
        }

        public async Task<bool> BookSeatAsync(int seatId, string userId)
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

        public async Task<bool> CancelSeatAsync(int seatId, string userId)
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

        // ── Code generator ───────────────────────────────────────────────────

        private async Task<string> GenerateUniqueCode(string prefix, Func<Ticket, string?> selector)
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
    }
}