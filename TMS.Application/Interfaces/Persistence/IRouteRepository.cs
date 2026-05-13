// TMS.Application/Interfaces/Persistence/IRouteRepository.cs
using TMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Persistence
{
    public interface IRouteRepository : IGenericRepository<Route, int>
    {
        Task<Route?> GetByCodeAsync(string routeCode);
        Task<IEnumerable<Route>> GetRoutesByLocationAsync(string departureLocationCode, string destinationLocationCode);
    }
}
