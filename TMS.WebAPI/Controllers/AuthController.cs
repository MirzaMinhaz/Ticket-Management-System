using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
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

        // Bootstrap-only: creates an Admin account with NO auth check.
        // This is intentionally dangerous — anyone who can reach this endpoint
        // gets a full Admin account. Use it once to create your first Admin,
        // then remove this action (or comment it out / gate it behind a
        // config flag like "AllowBootstrapAdminRegistration") before going live.
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            _logger.LogInformation("Attempting Admin (bootstrap) registration for Email: {Email}", request.Email);

            try
            {
                var response = await _authService.RegisterAsync(request);
                _logger.LogInformation("Admin registered successfully for Email: {Email}", request.Email);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning("Admin registration failed for Email: {Email}. Reason: {ErrorMessage}", request.Email, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Admin registration for Email: {Email}", request.Email);
                return StatusCode(500, "Registration failed due to an internal error");
            }
        }

        // Customer portal registration → Role: Customer. Public/self-serve, unchanged.
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
                _logger.LogWarning("Customer registration failed for Email: {Email}. Reason: {ErrorMessage}", request.Email, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Customer registration for Email: {Email}", request.Email);
                return StatusCode(500, "Registration failed due to an internal error");
            }
        }

        // Staff registration (Manager / Station / Counter) → only an existing
        // Admin can create these accounts, and Role is validated server-side.
        [Authorize(Roles = "Admin")]
        [HttpPost("register-staff")]
        public async Task<IActionResult> RegisterStaff(RegisterStaffRequestDto request)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation(
                "Admin {AdminId} attempting to register staff account for Email: {Email} with Role: {Role}",
                adminId, request.Email, request.Role);

            try
            {
                var response = await _authService.RegisterStaffAsync(request);
                _logger.LogInformation("Staff account registered successfully for Email: {Email} with Role: {Role}", request.Email, request.Role);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning("Staff registration failed for Email: {Email}. Reason: {ErrorMessage}", request.Email, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during staff registration for Email: {Email}", request.Email);
                return StatusCode(500, "Registration failed due to an internal error");
            }
        }

        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login rejected before authentication: missing username or password.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Login attempt for Username: {Username}", request.Username);

            try
            {
                var result = await _authService.LoginStaffAsync(request);
                _logger.LogInformation("User {Username} logged in successfully", request.Username);
                return Ok(result);
            }
            catch (UnauthorizedException)
            {
                // Already logged with full detail inside AuthService.AuthenticateAsync — don't duplicate here.
                return Unauthorized("Invalid credentials");
            }
            catch (ForbiddenException ex)
            {
                // Already logged with full detail inside AuthService.LoginStaffAsync — don't duplicate here.
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected system crash during login attempt for Username: {Username}", request.Username);
                return StatusCode(500, "Login failed due to a system error");
            }
        }

        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("login-customer")]
        public async Task<IActionResult> LoginCustomer(LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Customer login rejected before authentication: missing username or password.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Customer login attempt for Username: {Username}", request.Username);

            try
            {
                var result = await _authService.LoginAsync(request);
                _logger.LogInformation("Customer {Username} logged in successfully", request.Username);
                return Ok(result);
            }
            catch (UnauthorizedException)
            {
                // Already logged with detail inside AuthService.AuthenticateAsync — don't duplicate here.
                return Unauthorized("Invalid credentials");
            }
            catch (ForbiddenException ex)
            {
                // Already logged with detail inside AuthService.LoginAsync — don't duplicate here.
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected system crash during customer login attempt for Username: {Username}", request.Username);
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