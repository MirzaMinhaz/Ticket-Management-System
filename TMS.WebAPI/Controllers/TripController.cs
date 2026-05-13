// TMS.API.Controllers/TripController.cs
using Microsoft.AspNetCore.Mvc;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;
        public TripController(ITripService tripService) => _tripService = tripService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trips = await _tripService.GetAllAsync();
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trip = await _tripService.GetByIdAsync(id);
            return trip == null ? NotFound() : Ok(trip);
        }

        /// <summary>
        /// The key endpoint: given a scheduleId + date, return existing Trip or create a new one.
        /// Angular calls this BEFORE creating a ticket.
        /// POST /api/trip/find-or-create
        /// Body: { "scheduleId": 3, "tripDate": "2026-05-19T00:00:00" }
        /// </summary>
        [HttpPost("find-or-create")]
        public async Task<IActionResult> FindOrCreate([FromBody] CreateTripDto dto)
        {
            try
            {
                var trip = await _tripService.FindOrCreateTripAsync(dto.ScheduleId, dto.TripDate);
                return Ok(trip);
            } 
            catch (Exception ex)
            {
                return StatusCode(500, $"Error resolving trip: {ex.Message}");
            }
        }



        // TMS.API.Controllers/TripController.cs
        // Add this endpoint inside the existing TripController class

        /// <summary>
        /// Returns the list of seat numbers already booked for a given trip (Status = "Booked" only).
        /// GET /api/trip/{id}/booked-seats
        /// </summary>
        [HttpGet("{id}/booked-seats")]
        public async Task<IActionResult> GetBookedSeats(int id)
        {
            try
            {
                var bookedSeats = await _tripService.GetBookedSeatNumbersAsync(id);
                return Ok(bookedSeats); // returns string[] e.g. ["D1","D2","D3"]
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching booked seats: {ex.Message}");
            }
        }
    }
}