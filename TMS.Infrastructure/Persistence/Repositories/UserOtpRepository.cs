using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<UserOtp> GetLatestActiveOtpAsync(
            int userId,
            string otpType,
            CancellationToken cancellationToken = default)
        {
            return await _context.UserOtps
                .Where(x =>
                    x.UserId == userId &&
                    x.OtpType == otpType &&
                    !x.IsUsed)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}