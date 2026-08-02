using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        public AuthService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
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

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            // Credentials ভুল হলে সরাসরি UnauthorizedException থ্রো হবে (কোনো catch ব্লক দ্বারা Wrapped হবে না)
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid credentials");
            }

            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = GenerateJwtToken(user)
            };
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