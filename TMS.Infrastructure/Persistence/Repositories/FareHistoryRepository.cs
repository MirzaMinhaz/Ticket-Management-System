using Microsoft.EntityFrameworkCore;
using TMS.Application.Interfaces.Repositories;
using TMS.Domain.Entities;
using TMS.Infrastructure.Persistence;

namespace TMS.Infrastructure.Repositories
{
    public class FareHistoryRepository : IFareHistoryRepository
    {
        private readonly TicketManagementDbContext _context;

        public FareHistoryRepository(TicketManagementDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FareHistory entry)
        {
            await _context.FareHistories.AddAsync(entry);   // saved later by IUnitOfWork.CompleteAsync()
        }

        public async Task<List<FareHistory>> GetRecentAsync(int take)
        {
            return await _context.FareHistories
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .ToListAsync();
        }
    }
}