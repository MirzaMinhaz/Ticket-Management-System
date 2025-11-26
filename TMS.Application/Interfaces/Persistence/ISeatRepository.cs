using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces.Persistence
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>> GetSeatsByScheduleAsync(int scheduleId);
        Task<Seat> GetSeatByIdAsync(int seatId);
        Task UpdateSeatAsync(Seat seat);
        Task AddSeatAsync(Seat seat);
    }
}
