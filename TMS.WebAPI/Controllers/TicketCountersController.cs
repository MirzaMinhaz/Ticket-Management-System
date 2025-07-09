// TMS.WebAPI/Controllers/TicketCountersController.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs; // Ensure TicketCounter DTOs are available
using TMS.Application.Interfaces.Services;
using TMS.Application.Exceptions;

namespace TMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TicketCountersController : ControllerBase
    {
        private readonly ITicketCounterService _ticketCounterService;

        public TicketCountersController(ITicketCounterService ticketCounterService)
        {
            _ticketCounterService = ticketCounterService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TicketCounterDto>), 200)]
        public async Task<ActionResult<IEnumerable<TicketCounterDto>>> Get()
        {
            var ticketCounters = await _ticketCounterService.GetAllTicketCountersAsync();
            return Ok(ticketCounters);
        }

        // <<<--- NEW ENDPOINT FOR GETTING COUNTERS BY LOCATION ID
        [HttpGet("byLocation/{locationId}")]
        [ProducesResponseType(typeof(IEnumerable<TicketCounterDto>), 200)]
        [ProducesResponseType(404)] // Or 200 with empty list if no counters
        public async Task<ActionResult<IEnumerable<TicketCounterDto>>> GetTicketCountersByLocation(int locationId)
        {
            var ticketCounters = await _ticketCounterService.GetTicketCountersByLocationAsync(locationId);
            if (ticketCounters == null || !((List<TicketCounterDto>)ticketCounters).Any()) // Check if the list is empty
            {
                // Return 200 OK with an empty list, as it's not an error if a location has no counters
                return Ok(new List<TicketCounterDto>());
            }
            return Ok(ticketCounters);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TicketCounterDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TicketCounterDto>> Get(int id)
        {
            try
            {
                var ticketCounter = await _ticketCounterService.GetTicketCounterByIdAsync(id);
                return Ok(ticketCounter);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("code/{counterCode}")]
        [ProducesResponseType(typeof(TicketCounterDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TicketCounterDto>> GetByCode(string counterCode)
        {
            try
            {
                var ticketCounter = await _ticketCounterService.GetTicketCounterByCodeAsync(counterCode);
                return Ok(ticketCounter);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(TicketCounterDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<TicketCounterDto>> Post([FromBody] CreateTicketCounterDto createDto) // Assuming this DTO exists
        {
            try
            {
                var createdTicketCounter = await _ticketCounterService.CreateTicketCounterAsync(createDto);
                return CreatedAtAction(nameof(Get), new { id = createdTicketCounter.Id }, createdTicketCounter);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateTicketCounterDto updateDto) // Assuming this DTO exists
        {
            if (updateDto == null)
            {
                return BadRequest("Update data is null.");
            }

            try
            {
                await _ticketCounterService.UpdateTicketCounterAsync(id, updateDto);
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
                await _ticketCounterService.DeleteTicketCounterAsync(id);
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
    }
}