// TMS.Infrastructure/Persistence/Repositories/LocationRepository.cs
using TMS.Domain.Entities;
using TMS.Application.Interfaces.Persistence;
using TMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class LocationRepository : GenericRepository<Location, int>, ILocationRepository // CRITICAL: int ID
    {
        private readonly TicketManagementDbContext _dbContext;

        public LocationRepository(TicketManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Location?> GetByCodeAsync(string code)
        {
            return await _dbContext.Locations
                .FirstOrDefaultAsync(l => l.LocationCode == code);
        }

        public async Task DeleteAsync(int id) // CRITICAL: int ID
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task UpdateLocationAsync(Location location)
        {
            _dbSet.Update(location);
        }

        public async Task<IEnumerable<Location>> GetWhereAsync(Expression<Func<Location, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}