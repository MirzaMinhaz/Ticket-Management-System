using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TMS.Application.DTOs.Operator;
using TMS.Application.Interfaces.Services;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OperatorsController : ControllerBase
    {
        private readonly IOperatorService _service;
        private readonly ILogger<OperatorsController> _logger;

        public OperatorsController(IOperatorService service, ILogger<OperatorsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch operators");
                return StatusCode(500, new { message = "Failed to load operators." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch operator {Id}", id);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<OperatorDto>> Create([FromBody] CreateOperatorDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create operator");
                return StatusCode(500, new { message = "An unexpected error occurred while creating the operator." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOperatorDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update operator {Id}", id);
                return StatusCode(500, new { message = "Failed to update operator." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete operator {Id}", id);
                return StatusCode(500, new { message = "Failed to delete operator." });
            }
        }
    }
}