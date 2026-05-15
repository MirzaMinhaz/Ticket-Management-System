using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMapper _mapper;

        public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
        }


        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
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
                OperatorCode = vehicle.OperatorCode, // ✅ Updated,
                Type = vehicle.Type,
                Model = vehicle.Model,
                LicensePlate = vehicle.LicensePlate,
                Capacity = vehicle.Capacity,
                VehicleCode = vehicle.VehicleCode,
                CreatedAt = vehicle.CreatedAt,
                LastModifiedAt = vehicle.LastModifiedAt ?? DateTime.UtcNow
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
                OperatorCode = vehicle.OperatorCode,
                Type = vehicle.Type,
                Model = vehicle.Model,
                LicensePlate = vehicle.LicensePlate,
                Capacity = vehicle.Capacity,
                VehicleCode = vehicle.VehicleCode,
                CreatedAt = vehicle.CreatedAt,
                LastModifiedAt = vehicle.LastModifiedAt ?? DateTime.UtcNow
            };
        }

        public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto createDto)
        {
            var vehicle = new Vehicle
            {
                OperatorCode = createDto.OperatorCode, // ✅ explicitly null
                Type = createDto.Type,
                Model = createDto.Model,
                LicensePlate = createDto.LicensePlate,
                Capacity = createDto.Capacity,
                ACType = createDto.ACType,
                BusCategory = createDto.BusCategory,
                DeckLevel = createDto.DeckLevel,
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
                OperatorCode = vehicle.OperatorCode,
                Type = vehicle.Type,
                Model = vehicle.Model,
                LicensePlate = vehicle.LicensePlate,
                Capacity = vehicle.Capacity,
                VehicleCode = vehicle.VehicleCode,
                ACType = vehicle.ACType,
                BusCategory = vehicle.BusCategory,
                DeckLevel = vehicle.DeckLevel,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = vehicle.LastModifiedAt ?? DateTime.UtcNow
            };
        }

        public async Task UpdateVehicleAsync(int id, UpdateVehicleDto updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto), "Update data cannot be null.");

            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
                throw new NotFoundException($"Vehicle with ID {id} not found.");

            // Update fields
            vehicle.OperatorCode = updateDto.OperatorCode; // Can be null
            vehicle.Type = updateDto.Type;
            vehicle.Model = updateDto.Model;
            vehicle.LicensePlate = updateDto.LicensePlate;
            vehicle.Capacity = updateDto.Capacity;
            vehicle.LastModifiedAt = DateTime.UtcNow;

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
