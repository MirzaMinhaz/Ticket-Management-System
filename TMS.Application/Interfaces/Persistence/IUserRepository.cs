using System.Threading.Tasks;
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces.Persistence
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username); // ✅ New method
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}
