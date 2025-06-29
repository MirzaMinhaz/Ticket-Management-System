using TMS.Domain.Entities;

namespace TMS.Application.Interfaces.Persistence
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        // Add any Location-specific query or command methods here if needed, e.g.:
        // Task<Location?> GetLocationByNameAsync(string name);
    }
}