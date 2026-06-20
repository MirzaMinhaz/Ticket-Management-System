// TMS.API.Controllers/TicketController.cs  (updated)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Application.DTOs.Ticket;
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

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var ticket = await _ticketService.GetTicketByIdAsync(id);
        //    return ticket == null ? NotFound() : Ok(ticket);
        //}

        // ── New: must come BEFORE {id:int} below in source order for clarity,
        // though attribute routing in .NET 6+ resolves literal segments correctly either way ──
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

        [HttpGet("{id:int}")]   // ← added :int constraint to avoid clashing with "/my"
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            return ticket == null ? NotFound() : Ok(ticket);
        }

        [Authorize]   // ← must be logged in now, so we can read the userId claim
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
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch.");
            var updated = await _ticketService.UpdateTicketAsync(dto);
            return updated == null ? NotFound() : Ok(updated);
        }



        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketDto dto)
        //{
        //    if (id != dto.Id) return BadRequest("ID mismatch.");
        //    var updated = await _ticketService.UpdateTicketAsync(dto);
        //    return updated == null ? NotFound() : Ok(updated);
        //}

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

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var result = await _ticketService.DeleteTicketAsync(id);
        //    return result ? NoContent() : NotFound();
        //}

        /// <summary>
        /// Soft-cancel: marks ticket as Cancelled, frees the seat, keeps the row.
        /// PATCH /api/ticket/{id}/cancel
        /// Body: { "id": 5, "reason": "Passenger request" }
        /// </summary>
        //[HttpPatch("{id}/cancel")]
        //public async Task<IActionResult> Cancel(int id, [FromBody] CancelTicketDto dto)
        //{
        //    if (id != dto.Id) return BadRequest("ID mismatch.");
        //    var result = await _ticketService.CancelTicketAsync(dto);
        //    return result == null ? NotFound() : Ok(result);
        //}
    }
}