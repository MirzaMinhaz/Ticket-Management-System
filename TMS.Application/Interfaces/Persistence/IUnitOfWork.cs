// TMS.Application/Interfaces/Persistence/IUnitOfWork.cs
using System;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        ILocationRepository Locations { get; }
        ITicketCounterRepository TicketCounters { get; }
        Task<int> CompleteAsync();
    }
}