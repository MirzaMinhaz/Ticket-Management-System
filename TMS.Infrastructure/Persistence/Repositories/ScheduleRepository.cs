// TMS.Infrastructure/Repositories/ScheduleRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TMS.Application.Interfaces.Repositories;
using TMS.Domain.Entities;
using TMS.Infrastructure.Persistence;

namespace TMS.Infrastructure.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly TicketManagementDbContext _context;

        public ScheduleRepository(TicketManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Schedule> GetByIdAsync(int id)
        {
            return await _context.Schedules
                .Include(s => s.Route)
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Route)
                .Include(s => s.Vehicle)
                .ToListAsync();
        }


        public async Task AddAsync(Schedule schedule)
        {
            try
            {
                await _context.Schedules.AddAsync(schedule);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // EF Core-specific error (constraint violations, etc.)
                Console.Error.WriteLine($"[AddAsync] Database update error: {ex.Message}");
                throw new ApplicationException("Failed to save schedule to the database.", ex);
            }
            catch (Exception ex)
            {
                // Generic fallback
                Console.Error.WriteLine($"[AddAsync] Unexpected error: {ex.Message}");
                throw new ApplicationException("Unexpected error occurred while adding schedule.", ex);
            }
        }


        public async Task UpdateAsync(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
        }
    }
}
