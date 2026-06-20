// TMS.Application.Interfaces/ITicketService.cs
using TMS.Application.DTOs.Ticket;
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetAllTicketsAsync();
        Task<TicketDto?> GetTicketByIdAsync(int id);
        //Task<TicketDto> CreateTicketAsync(CreateTicketDto dto);
        Task<TicketDto> CreateTicketAsync(CreateTicketDto dto, int? userId);
        Task<TicketDto?> UpdateTicketAsync(UpdateTicketDto dto);
        Task<bool> DeleteTicketAsync(int id);
        Task<TicketDto?> CancelTicketAsync(CancelTicketDto dto);   // ← new
        Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(int userId);

        // Legacy seat helpers (keep if still used elsewhere)
        Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int scheduleId);
        Task<bool> BookSeatAsync(int seatId, string userId);
        Task<bool> CancelSeatAsync(int seatId, string userId);

    }
}