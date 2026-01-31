// TMS.Application/Interfaces/Persistence/IRouteRepository.cs
using TMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Persistence
{
    public interface IRouteRepository : IGenericRepository<Route, int> // CRITICAL: TId is int
    {
        Task<IEnumerable<Route>> GetRoutesByLocationAsync(string departureLocationCode, string destinationLocationCode);

        Task<Route> GetRouteByCodeAsync(string routeCode);
        // Any specific methods for Route beyond generic ones
    }
}
