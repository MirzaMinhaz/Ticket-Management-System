using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TMS.Application.Interfaces.Persistence;
using TMS.Infrastructure.Persistence;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId>
        where TEntity : class
    {
        protected readonly TicketManagementDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(TicketManagementDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<TEntity?> GetByIdAsync(TId id)
            => await _dbSet.FindAsync(id);

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync(); // 🔥 CRITICAL: Actually saves to DB
        }

        public async Task UpdateAsync(TEntity entity) // Changed to Async to support SaveChanges
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync(); // 🔥 CRITICAL: Actually saves to DB
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync(); // 🔥 CRITICAL: Actually saves to DB
        }

        // Methods below are for reading, so they don't need SaveChanges
        public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.SingleOrDefaultAsync(predicate);

        // Required if your interface still uses the non-async Update signature
        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
            _dbContext.SaveChanges(); // Synchronous save
        }
    }
}