using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces.Services
{
    public interface ITicketCounterService
    {
        Task<IEnumerable<TicketCounterDto>> GetAllTicketCountersAsync();
        Task<TicketCounterDto> GetTicketCounterByIdAsync(Guid ticketCounterId);
        Task<TicketCounterDto> CreateTicketCounterAsync(CreateTicketCounterDto ticketCounterDto);
        Task UpdateTicketCounterAsync(Guid ticketCounterId, UpdateTicketCounterDto ticketCounterDto);
        Task DeleteTicketCounterAsync(Guid ticketCounterId);
        Task<IEnumerable<TicketCounterDto>> GetTicketCountersByLocationAsync(Guid locationId);
    }
}