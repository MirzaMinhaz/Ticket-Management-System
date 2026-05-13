// TMS.Application.Interfaces.Repositories/ITripRepository.cs
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces.Repositories
{
    public interface ITripRepository
    {
        Task<Trip?> GetByScheduleAndDateAsync(int scheduleId, DateTime date);
        Task<Trip?> GetByIdAsync(int id);
        Task<IEnumerable<Trip>> GetAllAsync();
        Task AddAsync(Trip trip);
    }
}