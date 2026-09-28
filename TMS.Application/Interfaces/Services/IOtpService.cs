using System.Threading;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Services
{
    public interface IOtpService
    {
        Task<string> GenerateAndSaveCustomerOtpAsync(
            string username,
            CancellationToken cancellationToken = default);
    }
}