using TMS.Domain.Entities;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Repositories;
using TMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : GenericRepository<Ticket, int>, ITicketRepository
    {
        private readonly TicketManagementDbContext _dbContext;

        public TicketRepository(TicketManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Ticket?> GetTicketByCodeAsync(string code)
        {
            try
            {
                return await _dbSet.FirstOrDefaultAsync(t => t.TicketCode == code);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[GetTicketByCodeAsync] Error: {ex.Message}");
                throw;
            }
        }

        // ← This was the missing method causing CS0535
        public async Task<IEnumerable<Ticket>> GetByTripIdAsync(int tripId)
        {
            try
            {
                return await _dbSet
                    .Where(t => t.TripId == tripId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[GetByTripIdAsync] Error: {ex.Message}");
                throw;
            }
        }
    }
}