using AutoMapper;
using Microsoft.Extensions.Logging; // ILogger এর জন্য এটি প্রয়োজন
using System;
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
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper, ILogger<VehicleService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            try
            {
                var vehicles = await _vehicleRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all vehicles.");
                throw;
            }
        }

        public async Task<VehicleDto> GetVehicleByIdAsync(int id)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(id);
                if (vehicle == null)
                    throw new NotFoundException($"Vehicle with ID {id} not found.");

                return _mapper.Map<VehicleDto>(vehicle);
            }
            catch (NotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching vehicle with ID {Id}", id);
                throw;
            }
        }

        public async Task<VehicleDto> GetVehicleByCodeAsync(string vehicleCode)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByCodeAsync(vehicleCode);
                if (vehicle == null)
                    throw new NotFoundException($"Vehicle with code {vehicleCode} not found.");

                return _mapper.Map<VehicleDto>(vehicle);
            }
            catch (NotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching vehicle with code {Code}", vehicleCode);
                throw;
            }
        }

        public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto createDto)
        {
            try
            {
                var vehicle = new Vehicle
                {
                    OperatorCode = createDto.OperatorCode,
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
                return _mapper.Map<VehicleDto>(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating a new vehicle.");
                throw;
            }
        }

        public async Task UpdateVehicleAsync(int id, UpdateVehicleDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    throw new ArgumentNullException(nameof(updateDto));

                var vehicle = await _vehicleRepository.GetByIdAsync(id);
                if (vehicle == null)
                    throw new NotFoundException($"Vehicle with ID {id} not found.");

                // Map updates
                _mapper.Map(updateDto, vehicle);
                vehicle.LastModifiedAt = DateTime.UtcNow;

                await _vehicleRepository.UpdateAsync(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vehicle with ID {Id}", id);
                throw;
            }
        }

        public async Task DeleteVehicleAsync(int id)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(id);
                if (vehicle == null)
                    throw new NotFoundException($"Vehicle with ID {id} not found.");

                await _vehicleRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vehicle with ID {Id}", id);
                throw;
            }
        }

        private async Task<string> GenerateVehicleCodeAsync()
        {
            var allVehicles = await _vehicleRepository.GetAllAsync();
            var nextId = allVehicles.Any() ? allVehicles.Max(v => v.Id) + 1 : 1;
            return $"VHC-{nextId:D4}";
        }
    }
}