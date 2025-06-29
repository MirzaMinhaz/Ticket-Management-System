using AutoMapper;
using TMS.Application.DTOs;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities; // Needed for Location entity

namespace TMS.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public LocationService(IUnitOfWork unitOfWork, ILocationRepository locationRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public async Task<List<LocationDto>> GetAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            return _mapper.Map<List<LocationDto>>(locations);
        }

        public async Task<LocationDto?> GetLocationByIdAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<LocationDto> CreateLocationAsync(LocationDto locationDto)
        {
            // Application-specific validation before calling domain method
            // e.g., Check if a location with the same name already exists
            // if (await _locationRepository.GetLocationByNameAsync(locationDto.Name) != null)
            // {
            //     throw new ArgumentException("A location with this name already exists.");
            // }

            // Use the domain entity's factory method for controlled creation
            var location = Location.Create(
                locationDto.Name,
                locationDto.Type,
                locationDto.Address,
                locationDto.Latitude,
                locationDto.Longitude
            );

            await _locationRepository.AddAsync(location);
            await _unitOfWork.SaveChangeAsync(); // Commit transaction

            return _mapper.Map<LocationDto>(location);
        }

        public async Task UpdateLocationAsync(LocationDto locationDto)
        {
            // Retrieve existing entity from database
            var locationToUpdate = await _locationRepository.GetByIdAsync(locationDto.Id);
            if (locationToUpdate == null)
            {
                throw new KeyNotFoundException($"Location with ID {locationDto.Id} not found.");
            }

            // Use the domain entity's update method to apply changes
            locationToUpdate.Update(
                locationDto.Name,
                locationDto.Type,
                locationDto.Address,
                locationDto.Latitude,
                locationDto.Longitude
            );

            await _locationRepository.UpdateAsync(locationToUpdate); // Update tracked entity state (EF Core usually detects this)
            await _unitOfWork.SaveChangeAsync(); // Commit transaction
        }

        public async Task DeleteLocationAsync(Guid id)
        {
            var locationToDelete = await _locationRepository.GetByIdAsync(id);
            if (locationToDelete == null)
            {
                throw new KeyNotFoundException($"Location with ID {id} not found.");
            }

            await _locationRepository.DeleteAsync(locationToDelete);
            await _unitOfWork.SaveChangeAsync(); // Commit transaction
        }
    }
}