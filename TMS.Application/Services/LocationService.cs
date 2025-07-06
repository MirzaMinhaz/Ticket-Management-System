using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LocationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            var locations = await _unitOfWork.Locations.GetAllAsync();
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<LocationDto> GetLocationByIdAsync(Guid locationId)
        {
            var location = await _unitOfWork.Locations.GetByIdAsync(locationId);
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto locationDto)
        {
            var location = _mapper.Map<Location>(locationDto);
            // Id is handled by BaseEntity/EF Core. No need to set here.
            var newLocation = await _unitOfWork.Locations.AddAsync(location);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<LocationDto>(newLocation);
        }

        public async Task UpdateLocationAsync(Guid locationId, UpdateLocationDto locationDto)
        {
            var existingLocation = await _unitOfWork.Locations.GetByIdAsync(locationId);
            if (existingLocation == null)
            {
                // Optionally throw a custom not found exception
                throw new ApplicationException($"Location with ID {locationId} not found.");
            }

            // Map updated properties from DTO to existing entity
            _mapper.Map(locationDto, existingLocation);
            existingLocation.LastModifiedAt = DateTime.UtcNow; // Update timestamp

            await _unitOfWork.Locations.UpdateAsync(existingLocation);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteLocationAsync(Guid locationId)
        {
            var locationToDelete = await _unitOfWork.Locations.GetByIdAsync(locationId);
            if (locationToDelete == null)
            {
                throw new ApplicationException($"Location with ID {locationId} not found.");
            }

            await _unitOfWork.Locations.DeleteAsync(locationToDelete);
            await _unitOfWork.CompleteAsync();
        }
    }
}