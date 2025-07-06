using TMS.Application.Interfaces.Persistence;
using TMS.Domain.Entities;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        public LocationRepository(TicketManagementDbContext dbContext) : base(dbContext)
        {
        }

        // Location-specific repository methods can be added here
    }
}