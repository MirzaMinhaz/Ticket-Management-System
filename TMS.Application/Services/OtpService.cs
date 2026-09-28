using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using TMS.Application.Exceptions;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Constants;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserOtpRepository _userOtpRepository;

        public OtpService(
            IUserRepository userRepository,
            IUserOtpRepository userOtpRepository)
        {
            _userRepository = userRepository;
            _userOtpRepository = userOtpRepository;
        }

        public async Task<string> GenerateAndSaveCustomerOtpAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
                throw new UnauthorizedException(
                    "Invalid customer credentials.");

            if (user.Role != Roles.Customer)
                throw new ForbiddenException(
                    "Only customers can request customer OTP.");

            var otp = RandomNumberGenerator
                .GetInt32(100000, 1000000)
                .ToString();

            var otpHash = BCrypt.Net.BCrypt.HashPassword(otp);

            var now = DateTime.UtcNow;

            var userOtp = new UserOtp
            {
                UserId = user.Id,
                OtpHash = otpHash,
                OtpType = "CustomerLogin",
                ExpiresAt = now.AddMinutes(5),
                IsUsed = false,
                AttemptCount = 0,
                CreatedAt = now,
                CreatedBy = "System",
                LastModifiedAt = null,
                LastModifiedBy = "System"
            };

            await _userOtpRepository.AddAsync(
                userOtp,
                cancellationToken);

            await _userOtpRepository.SaveChangesAsync(
                cancellationToken);

            return otp;
        }
    }
}