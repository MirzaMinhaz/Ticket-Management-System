// TMS.Infrastructure/Persistence/Repositories/RouteRepository.cs
using TMS.Domain.Entities;
using TMS.Application.Interfaces.Persistence;
using TMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class RouteRepository : GenericRepository<Route, int>, IRouteRepository
    {
        private readonly TicketManagementDbContext _dbContext;

        public RouteRepository(TicketManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Route>> GetRoutesByLocationAsync(string departureLocationCode, string destinationLocationCode)
        {
            return await _dbContext.Routes
                .Where(r => r.DepartureLocationCode == departureLocationCode
                         && r.DestinationLocationCode == destinationLocationCode)
                .ToListAsync();
        }

        public async Task<Route> GetRouteByCodeAsync(string routeCode)
        {
            return await _dbContext.Routes
                .FirstOrDefaultAsync(r => r.RouteCode == routeCode);
        }
    }
}
