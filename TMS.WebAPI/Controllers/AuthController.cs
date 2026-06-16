using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMS.Application.DTOs;
using TMS.Application.Exceptions;
using TMS.Application.Interfaces.Services;

namespace TMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            return Ok("This is protected data only for Admins");
        }

        // Admin portal registration → Role: Admin
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Customer portal registration → Role: Customer
        [HttpPost("register-customer")]
        public async Task<IActionResult> RegisterCustomer(RegisterRequestDto request)
        {
            try
            {
                var response = await _authService.RegisterCustomerAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedException)
            {
                return Unauthorized("Invalid credentials"); // ✅ clean message
            }
            catch (Exception ex)
            {
                return BadRequest("Login failed"); // generic fallback
            }
        }


    }
}
