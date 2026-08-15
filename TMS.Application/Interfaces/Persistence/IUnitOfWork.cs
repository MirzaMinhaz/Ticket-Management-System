// TMS.Application/Interfaces/Persistence/IUnitOfWork.cs
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        ILocationRepository Locations { get; }
        ITicketCounterRepository TicketCounters { get; }
        Task<int> CompleteAsync();
        // IUnitOfWork.cs — add this method signature
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}