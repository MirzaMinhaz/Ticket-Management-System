// TMS.Application/Interfaces/Persistence/IGenericRepository.cs
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Persistence
{
    // Change TId to string (or keep it generic but all implementations will be string for now)
    public interface IGenericRepository<TEntity, TId> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(TId id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity); // Update typically doesn't need to be async or return value
        Task DeleteAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate);
    }
}