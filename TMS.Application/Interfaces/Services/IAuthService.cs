using System.Threading.Tasks;
using TMS.Application.DTOs;
namespace TMS.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> RegisterCustomerAsync(RegisterRequestDto request);
        Task<AuthResponseDto> RegisterStaffAsync(RegisterStaffRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<UserProfileResponseDto> GetProfileAsync(int userId);
    }
}