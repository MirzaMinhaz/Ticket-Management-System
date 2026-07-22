using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TMS.Application.DTOs;
using TMS.Application.Exceptions;
using TMS.Application.Interfaces.Services;
using System.Security.Claims;

namespace TMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Admin secure data accessed by Admin ID: {AdminId}", adminId);

            return Ok("This is protected data only for Admins");
        }

        // Admin portal registration → Role: Admin
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            _logger.LogInformation("Attempting Admin registration for Email: {Email}", request.Email);

            try
            {
                var response = await _authService.RegisterAsync(request);
                _logger.LogInformation("Admin registered successfully for Email: {Email}", request.Email);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                // Email or Username duplicate error
                _logger.LogWarning("Admin registration failed for Email: {Email}. Reason: {ErrorMessage}", request.Email, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Unexpected system error
                _logger.LogError(ex, "Unexpected error during Admin registration for Email: {Email}", request.Email);
                return StatusCode(500, "Registration failed due to an internal error");
            }
        }

        // Customer portal registration → Role: Customer
        [HttpPost("register-customer")]
        public async Task<IActionResult> RegisterCustomer(RegisterRequestDto request)
        {
            _logger.LogInformation("Attempting Customer registration for Email: {Email}", request.Email);

            try
            {
                var response = await _authService.RegisterCustomerAsync(request);
                _logger.LogInformation("Customer registered successfully for Email: {Email}", request.Email);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                // Email or Username duplicate error
                _logger.LogWarning("Customer registration failed for Email: {Email}. Reason: {ErrorMessage}", request.Email, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Unexpected system error
                _logger.LogError(ex, "Unexpected error during Customer registration for Email: {Email}", request.Email);
                return StatusCode(500, "Registration failed due to an internal error");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            _logger.LogInformation("Login attempt for Username: {Username}", request.Username);

            try
            {
                var result = await _authService.LoginAsync(request);
                _logger.LogInformation("User {Username} logged in successfully", request.Username);
                return Ok(result);
            }
            catch (UnauthorizedException)
            {
                // ✅ AuthService থেকে সরাসরি ফায়ার হওয়া কাস্টম এক্সেপশন ধরবে
                _logger.LogWarning("Invalid login credentials provided for Username: {Username}", request.Username);
                return Unauthorized("Invalid credentials");
            }
            catch (Exception ex)
            {
                // 🚨 শুধু সত্যিকারের সিস্টেম বা ডাটাবেজ ক্র্যাশ এখানে আসবে
                _logger.LogError(ex, "Unexpected system crash during login attempt for Username: {Username}", request.Username);
                return StatusCode(500, "Login failed due to a system error");
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Unauthorized access attempt to GetMe - Invalid or missing user token claim");
                return Unauthorized("Invalid token");
            }

            try
            {
                var profile = await _authService.GetProfileAsync(userId);
                _logger.LogInformation("Successfully retrieved profile for User ID: {UserId}", userId);
                return Ok(profile);
            }
            catch (NotFoundException)
            {
                _logger.LogWarning("Profile fetch failed. User ID {UserId} not found in database", userId);
                return NotFound("User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching profile for User ID: {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving user profile");
            }
        }
    }
}