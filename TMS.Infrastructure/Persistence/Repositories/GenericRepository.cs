// TMS.Infrastructure/Persistence/Repositories/GenericRepository.cs
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TMS.Application.Interfaces.Persistence;
using TMS.Domain.Entities;
using TMS.Infrastructure.Persistence; // For DbContext

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : class
    {
        protected readonly TicketManagementDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(TicketManagementDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(TId id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<Location> GetByCodeAsync(string locationCode)
        {
            return await _dbContext.Locations.FirstOrDefaultAsync(l => l.LocationCode == locationCode);
        }


        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            // No SaveChangesAsync here; UnitOfWork handles it
        }

        public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate);
        }
    }
}