using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Application.Interfaces.Persistence;
using TMS.Domain.Entities;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class TicketCounterRepository : GenericRepository<TicketCounter>, ITicketCounterRepository
    {
        public TicketCounterRepository(TicketManagementDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<TicketCounter>> GetCountersByLocationAsync(Guid locationId)
        {
            return await _dbSet
                .Where(tc => tc.LocationId == locationId)
                .OrderBy(tc => tc.CounterName)
                .ToListAsync();
        }

        public new async Task<TicketCounter> GetByIdAsync(Guid id)
        {
            // Override to include Location when fetching a single TicketCounter
            return await _dbSet
                .Include(tc => tc.Location)
                .FirstOrDefaultAsync(tc => tc.Id == id);
        }

        public new async Task<IReadOnlyList<TicketCounter>> GetAllAsync()
        {
            // Override to include Location when fetching all TicketCounters
            return await _dbSet
                .Include(tc => tc.Location)
                .ToListAsync();
        }
    }
}