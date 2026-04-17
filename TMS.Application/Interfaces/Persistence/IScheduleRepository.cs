// TMS.Application/Interfaces/Repositories/IScheduleRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces.Repositories
{
    public interface IScheduleRepository
    {
        Task<Schedule> GetByIdAsync(int id);
        Task<IEnumerable<Schedule>> GetAllAsync();
        Task AddAsync(Schedule schedule);
        Task UpdateAsync(Schedule schedule);
        Task DeleteAsync(int id);
    }
}
