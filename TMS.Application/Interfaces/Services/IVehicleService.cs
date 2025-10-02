using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
        Task<VehicleDto> GetVehicleByIdAsync(int id);
        Task<VehicleDto> GetVehicleByCodeAsync(string vehicleCode);
        Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto createDto);
        Task UpdateVehicleAsync(int id, UpdateVehicleDto updateDto);
        Task DeleteVehicleAsync(int id);
    }
}
