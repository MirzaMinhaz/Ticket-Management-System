using TMS.Domain.Entities;
using System.Threading.Tasks;
using TMS.Application.Interfaces.Persistence; // Needed for IGenericRepository

namespace TMS.Application.Interfaces.Persistence
{
    public interface ILocationRepository : IGenericRepository<Location, int>
    {
        Task<Location> GetLocationByCodeAsync(string locationCode);
    }
}