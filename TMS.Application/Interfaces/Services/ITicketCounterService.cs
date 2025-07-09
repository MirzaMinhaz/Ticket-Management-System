// TMS.Application/Interfaces/Services/ITicketCounterService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs; // Assuming TicketCounterDto exists

namespace TMS.Application.Interfaces.Services
{
    public interface ITicketCounterService
    {
        Task<IEnumerable<TicketCounterDto>> GetAllTicketCountersAsync();
        Task<TicketCounterDto> GetTicketCounterByIdAsync(int id);
        Task<TicketCounterDto> GetTicketCounterByCodeAsync(string counterCode);
        Task<IEnumerable<TicketCounterDto>> GetTicketCountersByLocationAsync(int locationId); // <<<--- ADDED THIS METHOD
        Task<TicketCounterDto> CreateTicketCounterAsync(CreateTicketCounterDto createDto);
        Task UpdateTicketCounterAsync(int id, UpdateTicketCounterDto updateDto);
        Task DeleteTicketCounterAsync(int id);
    }
}