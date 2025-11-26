// TMS.Application/Interfaces/ISeatService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces
{
    public interface ISeatService
    {
        Task<IEnumerable<SeatDto>> GetSeatsByScheduleAsync(int scheduleId);
        Task<SeatDto> GetSeatByIdAsync(int seatId);
        Task<bool> BookSeatAsync(int seatId, string userId);
        Task<bool> CancelSeatAsync(int seatId, string userId);
    }
}
