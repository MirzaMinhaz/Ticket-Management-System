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
        public RouteRepository(TicketManagementDbContext context) : base(context)
        {
        }

        public async Task<Route?> GetByCodeAsync(string routeCode)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.RouteCode == routeCode);
        }

        public async Task<IEnumerable<Route>> GetRoutesByLocationAsync(string departureLocationCode, string destinationLocationCode)
        {
            return await _dbSet
                .Where(r => r.DepartureLocationCode == departureLocationCode &&
                            r.DestinationLocationCode == destinationLocationCode)
                .ToListAsync();
        }
    }
}
