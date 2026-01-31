// TMS.Application/Interfaces/Services/IRouteService.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs.Route;

namespace TMS.Application.Interfaces.Services
{
    public interface IRouteService
    {
        Task<IEnumerable<RouteDto>> GetAllRoutesAsync();
        Task<RouteDto> GetRouteByIdAsync(int id);
        Task<RouteDto> CreateRouteAsync(CreateRouteDto createDto);
        Task UpdateRouteAsync(int id, UpdateRouteDto updateDto);
        Task DeleteRouteAsync(int id);

        // Extra methods for consistency
        Task<RouteDto> GetRouteByCodeAsync(string routeCode);
    }
}
