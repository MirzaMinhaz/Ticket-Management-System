using TMS.Domain.Entities;

namespace TMS.Application.Interfaces.Repositories
{
    public interface IFareHistoryRepository
    {
        Task AddAsync(FareHistory entry);
        Task<List<FareHistory>> GetRecentAsync(int take);
    }
}