using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Exceptions;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleService(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return vehicles.Select(v => new VehicleDto
            {
                Id = v.Id,
                OperatorId = v.OperatorId, // ✅ Safe: both sides nullable
                Type = v.Type,
                Model = v.Model,
                LicensePlate = v.LicensePlate,
                Capacity = v.Capacity,
                VehicleCode = v.VehicleCode,
                CreatedAt = v.CreatedAt,
                LastModifiedAt = v.LastModifiedAt
            }).ToList();
        }

        public async Task<VehicleDto> GetVehicleByIdAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
            {
                throw new NotFoundException($"Vehicle with ID {id} not found.");
            }

            return new VehicleDto
            {
                Id = vehicle.Id,
                OperatorId = vehicle.OperatorId, // ✅ Safe
                Type = vehicle.Type,
                Model = vehicle.Model,
                LicensePlate = vehicle.LicensePlate,
                Capacity = vehicle.Capacity,
                VehicleCode = vehicle.VehicleCode,
                CreatedAt = vehicle.CreatedAt,
                LastModifiedAt = vehicle.LastModifiedAt
            };
        }

        public async Task<VehicleDto> GetVehicleByCodeAsync(string vehicleCode)
        {
            var vehicle = await _vehicleRepository.GetByCodeAsync(vehicleCode);
            if (vehicle == null)
            {
                throw new NotFoundException($"Vehicle with code {vehicleCode} not found.");
            }

            return new VehicleDto
            {
                Id = vehicle.Id,
                OperatorId = vehicle.OperatorId,
                Type = vehicle.Type,
                Model = vehicle.Model,
                LicensePlate = vehicle.LicensePlate,
                Capacity = vehicle.Capacity,
                VehicleCode = vehicle.VehicleCode,
                CreatedAt = vehicle.CreatedAt,
                LastModifiedAt = vehicle.LastModifiedAt
            };
        }

        public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto createDto)
        {
            var vehicle = new Vehicle
            {
                OperatorId = null, // ✅ explicitly null
                Type = createDto.Type,
                Model = createDto.Model,
                LicensePlate = createDto.LicensePlate,
                Capacity = createDto.Capacity,
                VehicleCode = await GenerateVehicleCodeAsync(),
                CreatedBy = "system",
                CreatedAt = DateTime.UtcNow,
                LastModifiedBy = "system",
                LastModifiedAt = DateTime.UtcNow
            };

            await _vehicleRepository.AddAsync(vehicle);

            return new VehicleDto
            {
                Id = vehicle.Id,
                OperatorId = vehicle.OperatorId,
                Type = vehicle.Type,
                Model = vehicle.Model,
                LicensePlate = vehicle.LicensePlate,
                Capacity = vehicle.Capacity,
                VehicleCode = vehicle.VehicleCode,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = vehicle.LastModifiedAt
            };
        }

        public async Task UpdateVehicleAsync(int id, UpdateVehicleDto updateDto)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
            {
                throw new NotFoundException($"Vehicle with ID {id} not found.");
            }

            vehicle.OperatorId = updateDto.OperatorId;
            vehicle.Type = updateDto.Type;
            vehicle.Model = updateDto.Model;
            vehicle.LicensePlate = updateDto.LicensePlate;
            vehicle.Capacity = updateDto.Capacity;

            await _vehicleRepository.UpdateAsync(vehicle);
        }

        public async Task DeleteVehicleAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
            {
                throw new NotFoundException($"Vehicle with ID {id} not found.");
            }

            await _vehicleRepository.DeleteAsync(id);
        }

        private async Task<string> GenerateVehicleCodeAsync()
        {
            var allVehicles = await _vehicleRepository.GetAllAsync();
            var nextId = allVehicles.Any() ? allVehicles.Max(v => v.Id) + 1 : 1;
            return $"VHC-{nextId:D4}";
        }
    }
}
