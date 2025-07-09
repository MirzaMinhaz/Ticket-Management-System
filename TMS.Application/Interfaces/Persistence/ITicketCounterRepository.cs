// TMS.Application/Interfaces/Persistence/ITicketCounterRepository.cs
using TMS.Domain.Entities;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Persistence
{
    public interface ITicketCounterRepository : IGenericRepository<TicketCounter, int>
    {
        // Add specific methods for TicketCounter if needed, otherwise rely on GenericRepository
        // For example, if GetTicketCounterByCodeAsync isn't on IGenericRepository
        // Task<TicketCounter> GetTicketCounterByCodeAsync(string counterCode); // You can choose to add this or use FindSingleAsync
    }
}