using System.Linq.Expressions;

public interface IGenericRepository<TEntity, TId> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(TId id);

    Task AddAsync(TEntity entity);
    void Update(TEntity entity); // keep sync
    Task DeleteAsync(TEntity entity);

    Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate);
}