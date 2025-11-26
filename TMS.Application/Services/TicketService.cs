using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Application.Interfaces;
using TMS.Application.Interfaces.Persistence;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ISeatRepository _seatRepository;

        public TicketService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int scheduleId)
        {
            var seats = await _seatRepository.GetSeatsByScheduleAsync(scheduleId);
            return seats.Where(s => !s.IsBooked);
        }

        public async Task<bool> BookSeatAsync(int seatId, string userId)
        {
            var seat = await _seatRepository.GetSeatByIdAsync(seatId);
            if (seat == null || seat.IsBooked)
                return false;

            seat.IsBooked = true;
            seat.LastModifiedBy = userId;
            seat.LastModifiedAt = System.DateTime.UtcNow;

            await _seatRepository.UpdateSeatAsync(seat);
            return true;
        }

        public async Task<bool> CancelSeatAsync(int seatId, string userId)
        {
            var seat = await _seatRepository.GetSeatByIdAsync(seatId);
            if (seat == null || !seat.IsBooked)
                return false;

            seat.IsBooked = false;
            seat.LastModifiedBy = userId;
            seat.LastModifiedAt = System.DateTime.UtcNow;

            await _seatRepository.UpdateSeatAsync(seat);
            return true;
        }
    }
}
