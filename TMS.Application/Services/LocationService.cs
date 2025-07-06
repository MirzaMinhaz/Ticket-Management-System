// TMS.Application/Services/LocationService.cs
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Interfaces.Persistence; // Assuming this is where IUnitOfWork is
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities;
using System.Linq; // Needed for ToLower() if not already there
using Microsoft.EntityFrameworkCore; // Needed for AnyAsync if your repository uses it

namespace TMS.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork; // Your actual dependency
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
                throw new ApplicationException($"Location with ID {locationId} not found.");
            }

            _mapper.Map(locationDto, existingLocation);
            // existingLocation.LastModifiedAt = DateTime.UtcNow; // This is handled by DbContext SaveChanges override

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

        // MODIFIED: LocationExistsAsync to use _unitOfWork.Locations
        public async Task<bool> LocationExistsAsync(string name, string type)
        {
            // Assuming _unitOfWork.Locations is an IQueryable<Location> or has an AnyAsync method that accepts a predicate.
            // If your repository doesn't have an AnyAsync, you might need to add it to your IGenericRepository
            // or fetch all and filter in memory (less efficient for large datasets).
            return await _unitOfWork.Locations.AnyAsync(l =>
                l.Name.ToLower() == name.ToLower() &&
                l.Type.ToLower() == type.ToLower());
        }
    }
}