using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Interfaces.Services;

namespace TMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // GET: api/Locations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocations()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(locations);
        }

        // GET: api/Locations/5
        [HttpGet("{locationId}")] // Use locationId in route
        public async Task<ActionResult<LocationDto>> GetLocation(Guid locationId) // Use locationId parameter
        {
            var location = await _locationService.GetLocationByIdAsync(locationId);

            if (location == null)
            {
                return NotFound();
            }

            return Ok(location);
        }

        [HttpGet("exists")]
        public async Task<ActionResult<bool>> CheckLocationExists([FromQuery] string name, [FromQuery] string type)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type))
            {
                return BadRequest("Name and Type parameters are required.");
            }

            var exists = await _locationService.LocationExistsAsync(name, type);
            return Ok(exists);
        }

        // POST: api/Locations
        [HttpPost]
        public async Task<ActionResult<LocationDto>> PostLocation(CreateLocationDto locationDto)
        {
            var newLocation = await _locationService.CreateLocationAsync(locationDto);
            // Return 201 CreatedAtAction with the new resource's ID
            return CreatedAtAction(nameof(GetLocation), new { locationId = newLocation.LocationId }, newLocation);
        }

        // PUT: api/Locations/5
        [HttpPut("{locationId}")] // Use locationId in route
        public async Task<IActionResult> PutLocation(Guid locationId, UpdateLocationDto locationDto) // Use locationId parameter
        {
            // You might want to check if the ID in the DTO matches the route ID,
            // but for PUT, often the route ID is the authoritative one.

            try
            {
                await _locationService.UpdateLocationAsync(locationId, locationDto);
            }
            catch (ApplicationException ex) // Catch custom exception for not found
            {
                if (ex.Message.Contains("not found")) // A simple check, better with custom exception types
                {
                    return NotFound();
                }
                throw; // Re-throw other exceptions
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while updating the location.");
            }

            return NoContent(); // 204 No Content for successful update
        }

        // DELETE: api/Locations/5
        [HttpDelete("{locationId}")] // Use locationId in route
        public async Task<IActionResult> DeleteLocation(Guid locationId) // Use locationId parameter
        {
            try
            {
                await _locationService.DeleteLocationAsync(locationId);
            }
            catch (ApplicationException ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while deleting the location.");
            }

            return NoContent();
        }
    }
}