using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq; // For OrderByDescending etc in GenerateNextLocationCode if implemented here
using TMS.Application.Interfaces.Persistence;
using TMS.Domain.Entities;
using TMS.Infrastructure.Persistence; // Ensure DbContext is accessible

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class LocationRepository : GenericRepository<Location, int>, ILocationRepository
    {
        public LocationRepository(TicketManagementDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Location> GetLocationByCodeAsync(string locationCode)
        {
            return await _dbSet.FirstOrDefaultAsync(l => l.LocationCode == locationCode);
        }
    }
}