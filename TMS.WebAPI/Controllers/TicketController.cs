// TMS.API.Controllers/TicketController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Application.DTOs.Ticket;
using TMS.Application.Exceptions;
using TMS.Application.Interfaces;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService ticketService) => _ticketService = ticketService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTickets()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid token");

            var tickets = await _ticketService.GetTicketsByUserIdAsync(userId);
            return Ok(tickets);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            return ticket == null ? NotFound() : Ok(ticket);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int? userId = int.TryParse(userIdClaim, out var uid) ? uid : null;

                var created = await _ticketService.CreateTicketAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (SeatConflictException ex)
            {
                // 409 Conflict — the client should refresh its seat map, not
                // just show a generic error. Include the specific seats so
                // the frontend can highlight exactly what changed.
                return Conflict(new
                {
                    message = ex.Message,
                    conflictingSeats = ex.ConflictingSeats,
                });
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch.");
            try
            {
                var updated = await _ticketService.UpdateTicketAsync(dto);
                return updated == null ? NotFound() : Ok(updated);
            }
            catch (SeatConflictException ex)
            {
                return Conflict(new
                {
                    message = ex.Message,
                    conflictingSeats = ex.ConflictingSeats,
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ticketService.DeleteTicketAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelTicketDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch.");
            var result = await _ticketService.CancelTicketAsync(dto);
            return result == null ? NotFound() : Ok(result);
        }
    }
}