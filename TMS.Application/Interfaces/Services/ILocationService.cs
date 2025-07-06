using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
        Task<LocationDto> GetLocationByIdAsync(Guid locationId); // Renamed parameter
        Task<LocationDto> CreateLocationAsync(CreateLocationDto locationDto);
        Task UpdateLocationAsync(Guid locationId, UpdateLocationDto locationDto); // Renamed parameter
        Task DeleteLocationAsync(Guid locationId); // Renamed parameter
        Task<bool> LocationExistsAsync(string name, string type);
    }
}