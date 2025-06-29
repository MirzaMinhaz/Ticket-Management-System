using TMS.Application.DTOs;

namespace TMS.Application.Interfaces.Services
{
    public interface ILocationService
    {
        Task<List<LocationDto>> GetAllLocationsAsync();
        Task<LocationDto?> GetLocationByIdAsync(Guid id);
        Task<LocationDto> CreateLocationAsync(LocationDto locationDto);
        Task UpdateLocationAsync(LocationDto locationDto);
        Task DeleteLocationAsync(Guid id);
    }
}