// TMS.WebAPI/Controllers/RoutesController.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs.Route;
using TMS.Application.Interfaces.Services;
using TMS.Application.Exceptions;
using System.Linq;

namespace TMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RoutesController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RouteDto>), 200)]
        public async Task<ActionResult<IEnumerable<RouteDto>>> Get()
        {
            var routes = await _routeService.GetAllRoutesAsync();
            return Ok(routes);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RouteDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<RouteDto>> Get(int id)
        {
            try
            {
                var route = await _routeService.GetRouteByIdAsync(id);
                return Ok(route);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("code/{routeCode}")]
        [ProducesResponseType(typeof(RouteDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<RouteDto>> GetByCode(string routeCode)
        {
            try
            {
                var route = await _routeService.GetRouteByCodeAsync(routeCode);
                return Ok(route);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(RouteDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<RouteDto>> Create([FromBody] CreateRouteDto createDto)
        {
            try
            {
                var createdRoute = await _routeService.CreateRouteAsync(createDto);
                return CreatedAtAction(nameof(Get), new { id = createdRoute.Id }, createdRoute);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRouteDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest("Update data is null.");
            }

            try
            {
                await _routeService.UpdateRouteAsync(id, updateDto);
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
                await _routeService.DeleteRouteAsync(id);
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
