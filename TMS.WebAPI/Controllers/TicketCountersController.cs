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
    public class TicketCountersController : ControllerBase
    {
        private readonly ITicketCounterService _ticketCounterService;

        public TicketCountersController(ITicketCounterService ticketCounterService)
        {
            _ticketCounterService = ticketCounterService;
        }

        // GET: api/TicketCounters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketCounterDto>>> GetTicketCounters()
        {
            var ticketCounters = await _ticketCounterService.GetAllTicketCountersAsync();
            return Ok(ticketCounters);
        }

        // GET: api/TicketCounters/{ticketCounterId}
        [HttpGet("{ticketCounterId}")]
        public async Task<ActionResult<TicketCounterDto>> GetTicketCounter(Guid ticketCounterId)
        {
            var ticketCounter = await _ticketCounterService.GetTicketCounterByIdAsync(ticketCounterId);

            if (ticketCounter == null)
            {
                return NotFound();
            }

            return Ok(ticketCounter);
        }

        // GET: api/TicketCounters/ByLocation/{locationId}
        [HttpGet("ByLocation/{locationId}")]
        public async Task<ActionResult<IEnumerable<TicketCounterDto>>> GetTicketCountersByLocation(Guid locationId)
        {
            var ticketCounters = await _ticketCounterService.GetTicketCountersByLocationAsync(locationId);
            if (ticketCounters == null || !ticketCounters.Any())
            {
                return NotFound($"No ticket counters found for Location ID {locationId}.");
            }
            return Ok(ticketCounters);
        }


        // POST: api/TicketCounters
        [HttpPost]
        public async Task<ActionResult<TicketCounterDto>> PostTicketCounter(CreateTicketCounterDto ticketCounterDto)
        {
            var newTicketCounter = await _ticketCounterService.CreateTicketCounterAsync(ticketCounterDto);
            return CreatedAtAction(nameof(GetTicketCounter), new { ticketCounterId = newTicketCounter.TicketCounterId }, newTicketCounter);
        }

        // PUT: api/TicketCounters/{ticketCounterId}
        [HttpPut("{ticketCounterId}")]
        public async Task<IActionResult> PutTicketCounter(Guid ticketCounterId, UpdateTicketCounterDto ticketCounterDto)
        {
            try
            {
                await _ticketCounterService.UpdateTicketCounterAsync(ticketCounterId, ticketCounterDto);
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
                return StatusCode(500, "An error occurred while updating the ticket counter.");
            }

            return NoContent();
        }

        // DELETE: api/TicketCounters/{ticketCounterId}
        [HttpDelete("{ticketCounterId}")]
        public async Task<IActionResult> DeleteTicketCounter(Guid ticketCounterId)
        {
            try
            {
                await _ticketCounterService.DeleteTicketCounterAsync(ticketCounterId);
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
                return StatusCode(500, "An error occurred while deleting the ticket counter.");
            }

            return NoContent();
        }
    }
}