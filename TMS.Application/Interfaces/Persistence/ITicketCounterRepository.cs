using TMS.Domain.Entities;

namespace TMS.Application.Interfaces.Persistence
{
    public interface ITicketCounterRepository : IGenericRepository<TicketCounter>
    {
        // Add TicketCounter-specific methods if needed
        Task<IReadOnlyList<TicketCounter>> GetCountersByLocationAsync(Guid locationId);
    }
}