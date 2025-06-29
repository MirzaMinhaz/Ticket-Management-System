using TMS.Application.Interfaces.Persistence;
using TMS.Domain.Entities;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        public LocationRepository(TicketManagementDbContext dbContext) : base(dbContext)
        {
        }

        // Implement Location-specific methods defined in ILocationRepository here if any
        // Example:
        // public async Task<Location?> GetLocationByNameAsync(string name)
        // {
        //     return await _dbContext.Locations.FirstOrDefaultAsync(l => l.Name == name);
        // }
    }
}