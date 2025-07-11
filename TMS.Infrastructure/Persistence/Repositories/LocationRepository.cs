// TMS.Infrastructure/Persistence/Repositories/LocationRepository.cs
using TMS.Domain.Entities;
using TMS.Application.Interfaces.Persistence; // For ILocationRepository and IGenericRepository
using TMS.Infrastructure.Persistence; // For TicketManagementDbContext
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TMS.Infrastructure.Persistence.Repositories
{
    // CRITICAL: Inherit from GenericRepository<Location, int>
    public class LocationRepository : GenericRepository<Location, int>, ILocationRepository
    {
        private readonly TicketManagementDbContext _dbContext;

        public LocationRepository(TicketManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        // Add the missing method implementation:
        public async Task<Location> GetLocationByCodeAsync(string code)
        {
            // Use the LocationCode property from your Location entity
            return await _dbSet.FirstOrDefaultAsync(l => l.LocationCode == code);
        }

        // Ensure other specific methods of ILocationRepository (if any) are implemented here
        // For example, if ILocationRepository has Task DeleteAsync(int id)
        public async Task DeleteAsync(int id) // Ensure this matches ILocationRepository if it defines delete by ID
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                // SaveChangesAsync is handled by UnitOfWork, so no need to call _dbContext.SaveChangesAsync() here usually
            }
        }

        // Ensure UpdateLocationAsync (if defined in ILocationRepository) is implemented
        public async Task UpdateLocationAsync(Location location) // Example from previous response
        {
            _dbSet.Update(location);
            // SaveChangesAsync is handled by UnitOfWork
        }

        // Ensure GetWhereAsync (if defined in ILocationRepository) is implemented
        public async Task<IEnumerable<Location>> GetWhereAsync(Expression<Func<Location, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}