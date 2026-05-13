// TMS.Application.Interfaces.Repositories/ITicketRepository.cs
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<Ticket?> GetByIdAsync(int id);
        Task<IEnumerable<Ticket>> GetByTripIdAsync(int tripId);
        Task AddAsync(Ticket ticket);
        void Update(Ticket ticket);
        Task DeleteAsync(Ticket ticket);
    }
}