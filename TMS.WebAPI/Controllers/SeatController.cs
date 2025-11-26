// TMS.API/Controllers/SeatController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TMS.Application.Interfaces;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        [HttpGet("schedule/{scheduleId}")]
        public async Task<IActionResult> GetSeatsBySchedule(int scheduleId)
        {
            var seats = await _seatService.GetSeatsByScheduleAsync(scheduleId);
            return Ok(seats);
        }

        [HttpGet("{seatId}")]
        public async Task<IActionResult> GetSeatById(int seatId)
        {
            var seat = await _seatService.GetSeatByIdAsync(seatId);
            if (seat == null) return NotFound();
            return Ok(seat);
        }

        [HttpPost("book/{seatId}")]
        public async Task<IActionResult> BookSeat(int seatId, [FromQuery] string userId)
        {
            var result = await _seatService.BookSeatAsync(seatId, userId);
            if (!result) return BadRequest("Seat already booked or not found.");
            return Ok("Seat booked successfully.");
        }

        [HttpPost("cancel/{seatId}")]
        public async Task<IActionResult> CancelSeat(int seatId, [FromQuery] string userId)
        {
            var result = await _seatService.CancelSeatAsync(seatId, userId);
            if (!result) return BadRequest("Seat not booked or not found.");
            return Ok("Seat cancelled successfully.");
        }
    }
}
