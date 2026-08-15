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

        public async Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _dbSet
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[GetByUserIdAsync] Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Reads active (non-cancelled) seat numbers for a trip using
        /// AsNoTracking so this can be re-checked cheaply inside a transaction
        /// right before insert/update, closing the race window as tightly as
        /// EF Core allows without a raw SQL row lock.
        /// </summary>
        public async Task<HashSet<string>> GetActiveBookedSeatNumbersAsync(int tripId, int? excludeTicketId = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(t => t.TripId == tripId && t.Status != "Cancelled");

            if (excludeTicketId.HasValue)
                query = query.Where(t => t.Id != excludeTicketId.Value);

            var seatCsvList = await query.Select(t => t.SeatNumber).ToListAsync();

            // SeatNumber is stored as a comma-separated string ("A1, A2, B3") —
            // flatten every ticket's seats into one set of individually-held seats.
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var csv in seatCsvList)
            {
                if (string.IsNullOrWhiteSpace(csv)) continue;
                foreach (var seat in csv.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    result.Add(seat);
                }
            }
            return result;
        }
    }
}