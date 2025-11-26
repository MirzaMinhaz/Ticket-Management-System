// TMS.Application/Services/SeatService.cs
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Application.Interfaces.Persistence;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;

        public SeatService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        private SeatDto MapToDto(Seat seat)
        {
            return new SeatDto
            {
                Id = seat.Id,
                //ScheduleId = seat.ScheduleId,
                SeatNumber = seat.SeatNumber,
                SeatCode = seat.SeatCode,
                IsBooked = seat.IsBooked,

                // Computed property
                //public string Status => IsBooked ? "reserved" : "available";
    };
        }

        public async Task<IEnumerable<SeatDto>> GetSeatsByScheduleAsync(int scheduleId)
        {
            var seats = await _seatRepository.GetSeatsByScheduleAsync(scheduleId);
            return seats.Select(MapToDto);
        }

        public async Task<SeatDto> GetSeatByIdAsync(int seatId)
        {
            var seat = await _seatRepository.GetSeatByIdAsync(seatId);
            return seat == null ? null : MapToDto(seat);
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
