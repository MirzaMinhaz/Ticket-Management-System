// TMS.Application.Interfaces.Repositories/ITicketRepository.cs
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<Ticket?> GetByIdAsync(int id);
        Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Ticket>> GetByTripIdAsync(int tripId);
        Task AddAsync(Ticket ticket);
        void Update(Ticket ticket);
        Task DeleteAsync(Ticket ticket);

        /// <summary>
        /// Returns all seat numbers currently held by active (non-cancelled) tickets
        /// for a trip, optionally excluding one ticket id (used during edit/update so
        /// a ticket doesn't conflict with its own previously-held seats).
        /// </summary>
        Task<HashSet<string>> GetActiveBookedSeatNumbersAsync(int tripId, int? excludeTicketId = null);
    }
}