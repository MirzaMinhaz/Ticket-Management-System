// TMS.WebAPI/Controllers/LocationsController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Application.DTOs;
using TMS.Application.Interfaces.Services; 
using TMS.Application.Services;
using TMS.Application.Exceptions;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetAllLocations()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(locations);
        }

        // GET: api/Locations/{id}
        // Error CS1503 (Line 33 was likely this method parameter or a call to it)
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LocationDto>> GetLocationById(int id) // Changed Guid to int
        {
            var location = await _locationService.GetLocationByIdAsync(id); // Passed int id
            if (location == null)
            {
                return NotFound($"Location with ID {id} not found.");
            }
            return Ok(location);
        }

        // GET: api/Locations/code/{locationCode} (Optional: if you want to expose by code)
        [HttpGet("code/{locationCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LocationDto>> GetLocationByCode(string locationCode)
        {
            var location = await _locationService.GetLocationByCodeAsync(locationCode);
            if (location == null)
            {
                return NotFound($"Location with Code {locationCode} not found.");
            }
            return Ok(location);
        }


        // POST: api/Locations
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LocationDto>> CreateLocation([FromBody] CreateLocationDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdLocation = await _locationService.CreateLocationAsync(createDto);

                // Error CS1061 (Line 61 was likely here: trying to access LocationId)
                // Use .Id now, and optionally LocationCode for human-readable URL if desired
                return CreatedAtAction(nameof(GetLocationById), new { id = createdLocation.Id }, createdLocation);
                // Alternative if you prefer returning by code:
                // return CreatedAtAction(nameof(GetLocationByCode), new { locationCode = createdLocation.LocationCode }, createdLocation);
            }
            catch (ApplicationException ex) // Catch specific application exceptions, e.g., for duplicates
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new location.");
            }
        }

        // PUT: api/Locations/{id}
        // Error CS1503 (Line 73 was likely this method parameter or a call to it)
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateLocationDto updateDto) // Changed Guid to int
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _locationService.UpdateLocationAsync(id, updateDto); // Passed int id
                return NoContent();
            }
            catch (NotFoundException ex) // Catch custom Not Found exception from service
            {
                return NotFound(ex.Message);
            }
            catch (ApplicationException ex) // Catch other application exceptions, e.g., for duplicates
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating location with ID {id}.");
            }
        }

        // DELETE: api/Locations/{id}
        // Error CS1503 (Line 98 was likely this method parameter or a call to it)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // In case of FK constraint violation
        public async Task<IActionResult> DeleteLocation(int id) // Changed Guid to int
        {
            try
            {
                await _locationService.DeleteLocationAsync(id); // Passed int id
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ApplicationException ex) // For FK constraint or business rule violations
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting location with ID {id}.");
            }
        }
    }
}