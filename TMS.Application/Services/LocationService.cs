// TMS.Application/Services/LocationService.cs
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Interfaces.Persistence; // Ensure this is correct for ILocationRepository and IUnitOfWork
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities; // For Location entity

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

        public async Task<LocationDto> GetLocationByIdAsync(int id) // Change to int
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
            // REMOVE the line that tries to assign ID here:
            // location.LocationId = Guid.NewGuid().ToString(); // OR any manual assignment like location.Id = 0;
            // The database will assign the 'Id' after SaveChanges.

            location.CreatedAt = DateTime.UtcNow;
            location.CreatedBy = "System";

            await _locationRepository.AddAsync(location);
            await _unitOfWork.CompleteAsync(); // This triggers database save and assigns the ID to location.Id

            return _mapper.Map<LocationDto>(location); // location.Id will now be populated
        }

        public async Task UpdateLocationAsync(int id, UpdateLocationDto updateDto) // Change to int
        {
            var existingLocation = await _locationRepository.GetByIdAsync(id);
            if (existingLocation == null)
            {
                throw new Exception($"Location with ID {id} not found.");
            }

            _mapper.Map(updateDto, existingLocation);
            existingLocation.LastModifiedAt = DateTime.UtcNow;
            existingLocation.LastModifiedBy = "System";

            _locationRepository.Update(existingLocation);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteLocationAsync(int id) // Change to int
        {
            var existingLocation = await _locationRepository.GetByIdAsync(id);
            if (existingLocation == null)
            {
                throw new Exception($"Location with ID {id} not found.");
            }
            await _locationRepository.DeleteAsync(existingLocation); // Pass the entity for deletion
            await _unitOfWork.CompleteAsync();
        }
    }
}