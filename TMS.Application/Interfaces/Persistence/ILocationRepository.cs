// TMS.Application/Interfaces/Persistence/ILocationRepository.cs
using TMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Persistence
{
    public interface ILocationRepository : IGenericRepository<Location, int> // CRITICAL: TId is int
    {
        Task<Location> GetLocationByCodeAsync(string code);
        Task DeleteAsync(int id); // CRITICAL: int ID
        Task UpdateLocationAsync(Location location);
        Task<IEnumerable<Location>> GetWhereAsync(Expression<Func<Location, bool>> predicate);
    }
}