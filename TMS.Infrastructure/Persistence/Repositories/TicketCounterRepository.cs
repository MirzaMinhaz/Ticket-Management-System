// TMS.Infrastructure/Persistence/Repositories/TicketCounterRepository.cs
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TMS.Application.Interfaces.Persistence;
using TMS.Domain.Entities;
using TMS.Infrastructure.Persistence;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class TicketCounterRepository : GenericRepository<TicketCounter, int>, ITicketCounterRepository
    {
        public TicketCounterRepository(TicketManagementDbContext dbContext) : base(dbContext)
        {
        }

        // If you chose to add GetTicketCounterByCodeAsync to the interface:
        // public async Task<TicketCounter> GetTicketCounterByCodeAsync(string counterCode)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(tc => tc.CounterCode == counterCode);
        // }
    }
}