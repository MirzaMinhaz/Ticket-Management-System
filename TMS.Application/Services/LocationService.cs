// TMS.Application/Services/LocationService.cs
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Exceptions;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        public async Task<LocationDto> GetLocationByIdAsync(int id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
            {
                throw new NotFoundException($"Location with ID {id} not found.");
            }
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<LocationDto> GetLocationByCodeAsync(string locationCode)
        {
            var location = await _locationRepository.GetLocationByCodeAsync(locationCode);
            if (location == null)
            {
                throw new NotFoundException($"Location with code '{locationCode}' not found.");
            }
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto createDto)
        {
            var existingLocationByNameType = await _locationRepository.FindSingleAsync(l => l.Name == createDto.Name && l.Type == createDto.Type);
            if (existingLocationByNameType != null)
            {
                throw new ApplicationException($"A location with the name '{createDto.Name}' and type '{createDto.Type}' already exists.");
            }

            string newLocationCode = await GenerateNextLocationCode();

            var location = _mapper.Map<Location>(createDto);
            location.LocationCode = newLocationCode;

            await _locationRepository.AddAsync(location);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<LocationDto>(location);
        }

        public async Task UpdateLocationAsync(int id, UpdateLocationDto updateDto)
        {
            var existingLocation = await _locationRepository.GetByIdAsync(id);
            if (existingLocation == null)
            {
                throw new NotFoundException($"Location with ID {id} not found.");
            }

            var duplicateLocation = await _locationRepository.FindSingleAsync(l =>
                l.Name == updateDto.Name &&
                l.Type == updateDto.Type &&
                l.Id != id);

            if (duplicateLocation != null)
            {
                throw new ApplicationException($"A location with the name '{updateDto.Name}' and type '{updateDto.Type}' already exists.");
            }

            _mapper.Map(updateDto, existingLocation);

            _locationRepository.Update(existingLocation);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteLocationAsync(int id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
            {
                throw new NotFoundException($"Location with ID {id} not found.");
            }

            await _locationRepository.DeleteAsync(location); // <<<--- CHANGED to await DeleteAsync
            await _unitOfWork.CompleteAsync();
        }

        private async Task<string> GenerateNextLocationCode()
        {
            string lastCode = null;
            var allLocations = await _locationRepository.GetAllAsync();
            if (allLocations != null && allLocations.Any())
            {
                lastCode = allLocations.OrderByDescending(l => l.LocationCode).FirstOrDefault()?.LocationCode;
            }

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastCode) && lastCode.StartsWith("LOC-"))
            {
                if (int.TryParse(lastCode.Substring(4), out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"LOC-{nextNumber:D3}";
        }
    }
}