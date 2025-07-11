// TMS.Application/Interfaces/Persistence/ILocationRepository.cs
using TMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Persistence
{
    public interface ILocationRepository : IGenericRepository<Location, int> // CRITICAL: Change to int
    {
        Task<Location> GetLocationByCodeAsync(string code);
        // Define other specific methods for Location if they exist here
        Task DeleteAsync(int id); // If you want to allow deletion by ID in repository
        Task UpdateLocationAsync(Location location);
        Task<IEnumerable<Location>> GetWhereAsync(Expression<Func<Location, bool>> predicate);
    }
}