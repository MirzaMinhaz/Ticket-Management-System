using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.Interfaces.Persistence;
using TMS.Domain.Entities;
using TMS.Infrastructure.Persistence;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly TicketManagementDbContext _context;

        public VehicleRepository(TicketManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles.ToListAsync();
        }

        public async Task<Vehicle> GetByIdAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public async Task<Vehicle> GetByCodeAsync(string vehicleCode)
        {
            return await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleCode == vehicleCode);
        }

        public async Task AddAsync(Vehicle entity)
        {
            try
            {
                _context.Vehicles.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ EF Core Save Error: {ex.Message}");
                Console.WriteLine($"🔍 Inner Exception: {ex.InnerException?.Message}");
                throw; // rethrow to bubble up to controller
            }
        }


        public async Task UpdateAsync(Vehicle entity)
        {
            
            try
            {
                _context.Vehicles.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ EF Core Save Error: {ex.Message}");
                Console.WriteLine($"🔍 Inner Exception: {ex.InnerException?.Message}");
                throw; // rethrow to bubble up to controller
            }
        }

        public async Task DeleteAsync(int id)
        {
            var vehicle = await GetByIdAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
        }
    }
}
