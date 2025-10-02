using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Exceptions;
using TMS.Application.Interfaces.Services;
using System.Linq;

namespace TMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VehicleDto>), 200)]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> Get()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VehicleDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<VehicleDto>> Get(int id)
        {
            try
            {
                var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
                return Ok(vehicle);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("code/{vehicleCode}")]
        [ProducesResponseType(typeof(VehicleDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<VehicleDto>> GetByCode(string vehicleCode)
        {
            try
            {
                var vehicle = await _vehicleService.GetVehicleByCodeAsync(vehicleCode);
                return Ok(vehicle);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        //[HttpPost]
        //[ProducesResponseType(typeof(VehicleDto), 201)]
        //[ProducesResponseType(400)]
        //public async Task<ActionResult<VehicleDto>> Post([FromBody] CreateVehicleDto createDto)
        //{
        //    try
        //    {
        //        var createdVehicle = await _vehicleService.CreateVehicleAsync(createDto);
        //        return CreatedAtAction(nameof(Get), new { id = createdVehicle.Id }, createdVehicle);
        //    }
        //    catch (ApplicationException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost]
        [ProducesResponseType(typeof(VehicleDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<VehicleDto>> Post([FromBody] CreateVehicleDto createDto)
        {
            try
            {
                var createdVehicle = await _vehicleService.CreateVehicleAsync(createDto);
                return CreatedAtAction(nameof(Get), new { id = createdVehicle.Id }, createdVehicle);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Application-level error during vehicle creation");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating vehicle");
                return BadRequest("Unexpected error occurred.");
            }
        }


        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateVehicleDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest("Update data is null.");
            }

            try
            {
                await _vehicleService.UpdateVehicleAsync(id, updateDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _vehicleService.DeleteVehicleAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
