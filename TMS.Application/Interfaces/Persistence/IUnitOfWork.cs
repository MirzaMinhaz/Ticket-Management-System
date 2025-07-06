using System;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        ILocationRepository Locations { get; }
        ITicketCounterRepository TicketCounters { get; } // NEW
        // Add other repositories here (e.g., IUserRepository, IVehicleRepository)

        Task<int> CompleteAsync(); // Save changes to the database
    }
}