// TMS.Infrastructure/Persistence/Repositories/UnitOfWork.cs
using TMS.Application.Interfaces.Persistence; // For IUnitOfWork, ILocationRepository, ITicketCounterRepository
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using TMS.Infrastructure.Persistence; // For TicketManagementDbContext

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly TicketManagementDbContext _dbContext; // Consistent naming
        private ILocationRepository _locationRepository;
        private ITicketCounterRepository _ticketCounterRepository;

        public UnitOfWork(TicketManagementDbContext dbContext) // Consistent naming
        {
            _dbContext = dbContext;
        }

        public ILocationRepository Locations
        {
            get { return _locationRepository ??= new LocationRepository(_dbContext); }
        }

        public ITicketCounterRepository TicketCounters
        {
            get { return _ticketCounterRepository ??= new TicketCounterRepository(_dbContext); }
        }

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