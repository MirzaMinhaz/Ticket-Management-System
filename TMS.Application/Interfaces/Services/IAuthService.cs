using System.Threading.Tasks;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> RegisterCustomerAsync(RegisterRequestDto request);
        Task<AuthResponseDto> RegisterStaffAsync(RegisterStaffRequestDto request);

        // Customer-facing login — any authenticated user is allowed through.
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);

        // Staff-portal login — authenticates the same way, but additionally
        // rejects any user whose Role is not one of the staff portal roles.
        Task<AuthResponseDto> LoginStaffAsync(LoginRequestDto request);

        Task<UserProfileResponseDto> GetProfileAsync(int userId);
    }
}