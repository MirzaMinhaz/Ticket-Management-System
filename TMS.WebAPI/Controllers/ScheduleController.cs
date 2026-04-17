using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs.Schedule;
using TMS.Application.Interfaces.Services;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleDto>> GetById(int id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
                return NotFound();

            return Ok(schedule);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetAll()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            return Ok(schedules);
        }

        [HttpPost]
        public async Task<ActionResult<ScheduleDto>> Create([FromBody] CreateScheduleDto createDto)
        {
            try
            {
                var createdSchedule = await _scheduleService.CreateScheduleAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = createdSchedule.Id }, createdSchedule);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the schedule.", detail = ex.Message });
            }
        }



        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateScheduleDto updateDto)
        {
            await _scheduleService.UpdateScheduleAsync(id, updateDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _scheduleService.DeleteScheduleAsync(id);
            return NoContent();
        }
    }
}
