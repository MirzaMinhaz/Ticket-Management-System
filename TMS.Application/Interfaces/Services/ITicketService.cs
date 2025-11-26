using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int scheduleId);
        Task<bool> BookSeatAsync(int seatId, string userId);
        Task<bool> CancelSeatAsync(int seatId, string userId);
    }
}
