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

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            return await RegisterWithRole(request, "Admin");
        }

        public async Task<AuthResponseDto> RegisterCustomerAsync(RegisterRequestDto request)
        {
            return await RegisterWithRole(request, "Customer");
        }

        private async Task<AuthResponseDto> RegisterWithRole(RegisterRequestDto request, string role)
        {
            try
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
            catch (Exception ex)
            {
                throw new ApplicationException($"Registration failed: {ex.Message}", ex);
            }
        }

        //public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        //{
        //    try
        //    {
        //        var user = await _userRepository.GetByEmailAsync(request.Email);
        //        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        //            throw new UnauthorizedException("Invalid credentials");

        //        return new AuthResponseDto
        //        {
        //            UserId = user.Id,
        //            Username = user.Username,
        //            Token = GenerateJwtToken(user)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        // log exception here if you have a logger
        //        throw new ApplicationException($"Login failed: {ex.Message}", ex);
        //    }
        //}

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(request.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    throw new UnauthorizedException("Invalid credentials");

                return new AuthResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Token = GenerateJwtToken(user)
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Login failed: {ex.Message}", ex);
            }
        }



        private string GenerateJwtToken(User user)
        {
            try
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Token generation failed: {ex.Message}", ex);
            }
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
