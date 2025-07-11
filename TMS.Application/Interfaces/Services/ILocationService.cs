// TMS.Application/Interfaces/Services/ILocationService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
        Task<LocationDto> GetLocationByIdAsync(int id); // Change to int
        Task<LocationDto> GetLocationByCodeAsync(string code);
        Task<LocationDto> CreateLocationAsync(CreateLocationDto createDto);
        Task UpdateLocationAsync(int id, UpdateLocationDto updateDto); // Change to int
        Task DeleteLocationAsync(int id); // Change to int
    }
}