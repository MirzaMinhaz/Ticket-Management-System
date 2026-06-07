using Microsoft.AspNetCore.Mvc;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Application.Interfaces.Repositories;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;
        private readonly ITicketService _ticketService;
        private readonly ITicketRepository _ticketRepository;

        public TripController(
            ITripService tripService,
            ITicketService ticketService,
            ITicketRepository ticketRepository)
        {
            _tripService = tripService;
            _ticketService = ticketService;
            _ticketRepository = ticketRepository;
        }

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

        [HttpGet("{tripId}/booked-seats")]
        public async Task<IActionResult> GetBookedSeats(int tripId)
        {
            try
            {
                var tickets = await _ticketRepository.GetByTripIdAsync(tripId);
                var bookedSeats = tickets
                    .Where(t => t.Status != "Cancelled")
                    .SelectMany(t => t.SeatNumber
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim()))
                    .Distinct()
                    .ToList();

                return Ok(bookedSeats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching booked seats: {ex.Message}");
            }
        }
    }
}
