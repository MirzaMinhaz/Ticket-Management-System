using TMS.Application.DTOs;
using System.Collections.Generic; // CONFIRMED for IEnumerable
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllLocationsAsync(); // CONFIRMED to match implementation
        Task<LocationDto> GetLocationByIdAsync(int id);
        Task<LocationDto> GetLocationByCodeAsync(string locationCode);
        Task<LocationDto> CreateLocationAsync(CreateLocationDto createDto);
        Task UpdateLocationAsync(int id, UpdateLocationDto updateDto);
        Task DeleteLocationAsync(int id);
    }
}