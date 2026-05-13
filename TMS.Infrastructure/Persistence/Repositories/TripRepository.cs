using TMS.Domain.Entities;
using TMS.Application.Interfaces.Repositories;
using TMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class TripRepository : GenericRepository<Trip, int>, ITripRepository
    {
        private readonly TicketManagementDbContext _dbContext;

        public TripRepository(TicketManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Trip?> GetByScheduleAndDateAsync(int scheduleId, DateTime date)
        {
            try
            {
                var dateOnly = date.Date;
                return await _dbSet.FirstOrDefaultAsync(
                    t => t.ScheduleId == scheduleId && t.TripDate == dateOnly
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[GetByScheduleAndDateAsync] Error: {ex.Message}");
                throw;
            }
        }

        public async Task<Trip?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet.FirstOrDefaultAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[GetByIdAsync] Error: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Trip>> GetAllAsync()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[GetAllAsync] Error: {ex.Message}");
                throw;
            }
        }

        public async Task AddAsync(Trip trip)
        {
            try
            {
                await _dbSet.AddAsync(trip);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[AddAsync] Error: {ex.Message}");
                throw;
            }
        }
    }
}