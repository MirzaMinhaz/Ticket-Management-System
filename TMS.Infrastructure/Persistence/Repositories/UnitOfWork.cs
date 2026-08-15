// TMS.Infrastructure/Persistence/Repositories/UnitOfWork.cs
using TMS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage; // <-- Add this for IDbContextTransaction
using System.Threading.Tasks;
using System;
using TMS.Infrastructure.Persistence;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly TicketManagementDbContext _dbContext;
        private ILocationRepository _locationRepository;
        private ITicketCounterRepository _ticketCounterRepository;

        public UnitOfWork(TicketManagementDbContext dbContext)
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

        // Add this implementation to fix CS0535
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}