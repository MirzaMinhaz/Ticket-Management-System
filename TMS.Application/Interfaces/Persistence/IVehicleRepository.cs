using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces.Persistence
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<Vehicle> GetByIdAsync(int id);
        Task<Vehicle> GetByCodeAsync(string vehicleCode);
        Task AddAsync(Vehicle entity);
        Task UpdateAsync(Vehicle entity);
        Task DeleteAsync(int id);
    }
}
