using System;
using System.Threading.Tasks;
using TMS.Application.Interfaces.Persistence;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TicketManagementDbContext _dbContext;
        private ILocationRepository _locationRepository;
        private ITicketCounterRepository _ticketCounterRepository; // NEW

        public UnitOfWork(TicketManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ILocationRepository Locations => _locationRepository ??= new LocationRepository(_dbContext);
        public ITicketCounterRepository TicketCounters => _ticketCounterRepository ??= new TicketCounterRepository(_dbContext); // NEW

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}