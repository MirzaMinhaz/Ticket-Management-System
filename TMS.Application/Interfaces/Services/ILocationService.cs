// TMS.Application/Interfaces/Services/ILocationService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
        Task<LocationDto> GetLocationByIdAsync(int id); // CRITICAL: int ID
        Task<LocationDto> GetLocationByCodeAsync(string code); // String code
        Task<LocationDto> CreateLocationAsync(CreateLocationDto createDto);
        Task UpdateLocationAsync(int id, UpdateLocationDto updateDto); // CRITICAL: int ID
        Task DeleteLocationAsync(int id); // CRITICAL: int ID
    }
}