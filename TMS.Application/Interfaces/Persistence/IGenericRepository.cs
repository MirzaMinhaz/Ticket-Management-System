// TMS.Application/Interfaces/Persistence/IGenericRepository.cs
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Persistence
{
    public interface IGenericRepository<TEntity, TId> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(TId id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        Task DeleteAsync(TEntity entity); // Method should be Task, as in implementation
    }
}