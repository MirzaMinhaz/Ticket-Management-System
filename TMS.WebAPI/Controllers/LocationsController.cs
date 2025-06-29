using Microsoft.AspNetCore.Mvc;
using TMS.Application.DTOs;
using TMS.Application.Interfaces.Services; // Add this using

namespace TMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // e.g., /api/Locations
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<LocationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(locations);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LocationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLocationById(Guid id)
        {
            var location = await _locationService.GetLocationByIdAsync(id);
            if (location == null)
            {
                return NotFound($"Location with ID {id} not found.");
            }
            return Ok(location);
        }

        [HttpPost]
        [ProducesResponseType(typeof(LocationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateLocation([FromBody] LocationDto locationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Basic model validation based on DTO attributes
            }

            try
            {
                var createdLocation = await _locationService.CreateLocationAsync(locationDto);
                // Return 201 Created and the location of the new resource
                return CreatedAtAction(nameof(GetLocationById), new { id = createdLocation.Id }, createdLocation);
            }
            catch (ArgumentException ex) // Catch domain/application validation errors
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the location.");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] LocationDto locationDto)
        {
            if (id != locationDto.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _locationService.UpdateLocationAsync(locationDto);
                return NoContent(); // 204 No Content for successful update
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the location.");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLocation(Guid id)
        {
            try
            {
                await _locationService.DeleteLocationAsync(id);
                return NoContent(); // 204 No Content for successful deletion
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the location.");
            }
        }
    }
}