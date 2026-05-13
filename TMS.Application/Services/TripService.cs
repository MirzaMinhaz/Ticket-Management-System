// TMS.Application.Services/TripService.cs
using AutoMapper;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Repositories;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // ── Single constructor (merged) ──────────────────────────────────────
        public TripService(
            ITripRepository tripRepository,
            ITicketRepository ticketRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _tripRepository = tripRepository;
            _ticketRepository = ticketRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TripDto> FindOrCreateTripAsync(int scheduleId, DateTime tripDate)
        {
            var dateOnly = tripDate.Date;
            var existing = await _tripRepository.GetByScheduleAndDateAsync(scheduleId, dateOnly);
            if (existing != null)
                return _mapper.Map<TripDto>(existing);

            var newTrip = new Trip
            {
                ScheduleId = scheduleId,
                TripDate = dateOnly,
                Status = "Open",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                LastModifiedAt = DateTime.UtcNow,
                LastModifiedBy = "System"
            };

            await _tripRepository.AddAsync(newTrip);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<TripDto>(newTrip);
        }

        public async Task<TripDto?> GetByIdAsync(int id)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            return trip == null ? null : _mapper.Map<TripDto>(trip);
        }

        public async Task<IEnumerable<TripDto>> GetAllAsync()
        {
            var trips = await _tripRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TripDto>>(trips);
        }

        public async Task<IEnumerable<string>> GetBookedSeatNumbersAsync(int tripId)
        {
            var tickets = await _ticketRepository.GetByTripIdAsync(tripId);
            return tickets
                .Where(t => t.Status == "Booked" && !string.IsNullOrWhiteSpace(t.SeatNumber))
                .SelectMany(t => t.SeatNumber!
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()))
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .ToList();
        }
    }
}