// TMS.Application/Services/LocationService.cs
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepository locationRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<LocationDto> GetLocationByIdAsync(int id) // CRITICAL: int ID
        {
            var location = await _locationRepository.GetByIdAsync(id);
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<LocationDto> GetLocationByCodeAsync(string code)
        {
            var location = await _locationRepository.GetLocationByCodeAsync(code);
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto createDto)
        {
            var location = _mapper.Map<Location>(createDto);

            // Location.Id (int) is auto-incremented by the database. DO NOT SET IT HERE.
            // Generate LocationCode (string)
            location.LocationCode = await GenerateUniqueLocationCode();

            location.CreatedAt = DateTime.UtcNow;
            location.CreatedBy = "SystemUser";
            location.LastModifiedAt = DateTime.UtcNow;
            location.LastModifiedBy = "SystemUser";

            await _locationRepository.AddAsync(location);
            await _unitOfWork.CompleteAsync(); // This saves to DB and populates `location.Id`

            return _mapper.Map<LocationDto>(location);
        }

        public async Task UpdateLocationAsync(int id, UpdateLocationDto updateDto) // CRITICAL: int ID
        {
            var existingLocation = await _locationRepository.GetByIdAsync(id);
            if (existingLocation == null)
            {
                throw new Exception($"Location with ID {id} not found.");
            }

            _mapper.Map(updateDto, existingLocation);
            existingLocation.LastModifiedAt = DateTime.UtcNow;
            existingLocation.LastModifiedBy = "SystemUser";

            _locationRepository.Update(existingLocation);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteLocationAsync(int id) // CRITICAL: int ID
        {
            var existingLocation = await _locationRepository.GetByIdAsync(id);
            if (existingLocation == null)
            {
                throw new Exception($"Location with ID {id} not found.");
            }
            await _locationRepository.DeleteAsync(existingLocation);
            await _unitOfWork.CompleteAsync();
        }

        private async Task<string> GenerateUniqueLocationCode()
        {
            var allLocations = await _locationRepository.GetAllAsync();
            string lastCode = allLocations
                                .Select(l => l.LocationCode)
                                .Where(code => code != null && code.StartsWith("LOC-"))
                                .OrderByDescending(code => code)
                                .FirstOrDefault();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastHyphenIndex = lastCode.LastIndexOf('-');
                if (lastHyphenIndex != -1 && lastCode.Length > lastHyphenIndex + 1)
                {
                    string numericPart = lastCode.Substring(lastHyphenIndex + 1);
                    if (int.TryParse(numericPart, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }
            }
            return $"LOC-{nextNumber:D3}"; // Formats as LOC-001, LOC-002, etc.
        }
    }
}