using System.Threading;
using System.Threading.Tasks;
using TMS.Application.Interfaces.Persistence;
using TMS.Domain.Entities;
using TMS.Infrastructure.Persistence;

namespace TMS.Infrastructure.Repositories
{
    public class UserOtpRepository : IUserOtpRepository
    {
        private readonly TicketManagementDbContext _context;

        public UserOtpRepository(TicketManagementDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            UserOtp userOtp,
            CancellationToken cancellationToken = default)
        {
            await _context.UserOtps.AddAsync(
                userOtp,
                cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}