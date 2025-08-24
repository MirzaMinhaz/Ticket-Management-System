// TMS.Application/Interfaces/Persistence/ITicketCounterRepository.cs
using TMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Persistence
{
    public interface ITicketCounterRepository : IGenericRepository<TicketCounter, int> // CRITICAL: TId is int
    {
        Task<IEnumerable<TicketCounter>> GetTicketCountersByLocationAsync(int locationId);
        Task<TicketCounter> GetTicketCounterByCodeAsync(string counterCode);
        // Any specific methods for TicketCounter beyond generic ones
    }
}