using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Exceptions;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Constants;
using TMS.Domain.Entities;


namespace TMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;
        private readonly ILoginAttemptTracker _loginAttemptTracker;

        // Roles allowed to log into the staff portal. Customers are
        // authenticated successfully but rejected here with ForbiddenException.
        private static readonly string[] StaffPortalRoles =
        {
            Roles.Admin,
            Roles.Manager,
            Roles.StationAgent,
            Roles.CounterAgent
        };

        public AuthService(IUserRepository userRepository, IConfiguration config, ILogger<AuthService> logger, ILoginAttemptTracker loginAttemptTracker)
        {
            _userRepository = userRepository;
            _config = config;
            _logger = logger;
            _loginAttemptTracker = loginAttemptTracker;
        }

        // NOTE: kept only for initial system bootstrap (creating the very first
        // Admin account before any Admin exists to use RegisterStaffAsync).
        // The controller endpoint for this should be removed or heavily
        // restricted (e.g. only enabled via a one-time setup flag) once your
        // first real Admin account exists — see AuthController.
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            return await RegisterWithRole(request, Roles.Admin);
        }

        public async Task<AuthResponseDto> RegisterCustomerAsync(RegisterRequestDto request)
        {
            return await RegisterWithRole(request, Roles.Customer);
        }

        public async Task<AuthResponseDto> RegisterStaffAsync(RegisterStaffRequestDto request)
        {
            if (!Roles.IsValidStaffRole(request.Role))
            {
                throw new ApplicationException(
                    $"Invalid role '{request.Role}'. Must be one of: {string.Join(", ", Roles.AssignableStaffRoles)}");
            }

            var registerRequest = new RegisterRequestDto
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password
            };

            return await RegisterWithRole(registerRequest, request.Role);
        }

        private async Task<AuthResponseDto> RegisterWithRole(RegisterRequestDto request, string role)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new ApplicationException("Email already exists.");

            var existingUsername = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUsername != null)
                throw new ApplicationException("Username already taken.");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = role,
                UserCode = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                LastModifiedAt = DateTime.UtcNow,
                LastModifiedBy = "System"
            };

            await _userRepository.AddAsync(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = GenerateJwtToken(user)
            };
        }

        // Shared credential verification. The EXCEPTION message shown to the
        // client is always the same generic "Invalid credentials" (so we never
        // leak whether a username exists) — but internally we log the precise
        // reason, so ops/devs can actually debug failed logins from the log file.
        private async Task<User> AuthenticateAsync(LoginRequestDto request, string portalTag)
        {
            // Lock check সবার আগে — DB query/BCrypt চালানোর আগেই
            if (_loginAttemptTracker.IsLocked(request.Username))
            {
                var remaining = _loginAttemptTracker.GetRemainingLockTime(request.Username) ?? TimeSpan.FromMinutes(5);
                _logger.LogWarning(
                    "[{Portal}] Login blocked for Username: {Username}. Account temporarily locked due to repeated failed attempts. Retry after: {RemainingSeconds}s",
                    portalTag, request.Username, (int)remaining.TotalSeconds);

                throw new AccountLockedException("Too many failed attempts. Please try again later.", remaining);
            }

            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _loginAttemptTracker.RegisterFailedAttempt(request.Username);

                if (user == null)
                {
                    _logger.LogWarning(
                        "[{Portal}] Login failed for Username: {Username}. Reason: No user found with this username.",
                        portalTag, request.Username);
                }
                else
                {
                    _logger.LogWarning(
                        "[{Portal}] Login failed for Username: {Username}. Reason: Password mismatch (UserId: {UserId}, Role: {Role}).",
                        portalTag, request.Username, user.Id, user.Role);
                }

                throw new UnauthorizedException("Invalid credentials");
            }

            _loginAttemptTracker.ResetAttempts(request.Username); // সফল লগইনে কাউন্টার রিসেট
            return user;
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = GenerateJwtToken(user)
            };
        }

        // Customer-facing login: credentials must be valid AND the user's role
        // must be Customer. Staff accounts are rejected here, mirroring
        // LoginStaffAsync's portal restriction — this is now enforced server-side,
        // not just in the Angular component.
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await AuthenticateAsync(request, "CustomerPortal");

            if (user.Role != Roles.Customer)
            {
                _logger.LogWarning(
                    "[CustomerPortal] Login rejected for Username: {Username} (UserId: {UserId}). Reason: Role '{Role}' is not permitted to access the customer portal.",
                    user.Username, user.Id, user.Role);

                throw new ForbiddenException(
                    $"Role '{user.Role}' is not permitted to access the customer portal.");
            }

            return BuildAuthResponse(user);
        }

        // Staff-portal login: credentials must be valid AND the user's role
        // must be one of the staff portal roles. Otherwise ForbiddenException
        // is thrown — a real, server-side, logged rejection, not a client-side
        // UI decision.
        public async Task<AuthResponseDto> LoginStaffAsync(LoginRequestDto request)
        {
            var user = await AuthenticateAsync(request, "StaffPortal");

            if (!StaffPortalRoles.Contains(user.Role))
            {
                _logger.LogWarning(
                    "[StaffPortal] Login rejected for Username: {Username} (UserId: {UserId}). Reason: Role '{Role}' is not permitted to access the staff portal.",
                    user.Username, user.Id, user.Role);

                throw new ForbiddenException(
                    $"Role '{user.Role}' is not permitted to access the staff portal.");
            }

            return BuildAuthResponse(user);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var jwtKey = _config["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Secret Key is not configured in appsettings.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserProfileResponseDto> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("User not found");

            return new UserProfileResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                UserCode = user.UserCode,
                CreatedAt = user.CreatedAt
            };
        }
    }
}