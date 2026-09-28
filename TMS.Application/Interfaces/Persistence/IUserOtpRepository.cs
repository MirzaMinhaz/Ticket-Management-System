using System.Threading;
using System.Threading.Tasks;
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces.Persistence
{
    public interface IUserOtpRepository
    {
        Task AddAsync(
            UserOtp userOtp,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}