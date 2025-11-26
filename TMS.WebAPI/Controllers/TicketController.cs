using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMS.Application.Interfaces;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("available/{scheduleId}")]
        public async Task<IActionResult> GetAvailableSeats(int scheduleId)
        {
            var seats = await _ticketService.GetAvailableSeatsAsync(scheduleId);
            return Ok(seats);
        }

        [HttpPost("book/{seatId}")]
        public async Task<IActionResult> BookSeat(int seatId, [FromQuery] string userId)
        {
            var result = await _ticketService.BookSeatAsync(seatId, userId);
            if (!result) return BadRequest("Seat already booked or not found.");
            return Ok("Seat booked successfully.");
        }

        [HttpPost("cancel/{seatId}")]
        public async Task<IActionResult> CancelSeat(int seatId, [FromQuery] string userId)
        {
            var result = await _ticketService.CancelSeatAsync(seatId, userId);
            if (!result) return BadRequest("Seat not booked or not found.");
            return Ok("Seat cancelled successfully.");
        }
    }
}
