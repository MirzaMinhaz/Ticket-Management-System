// TMS.Application/Interfaces/Persistence/ITicketRepository.cs
using TMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TMS.Application.Interfaces.Persistence
{
    public interface ITicketRepository : IGenericRepository<Ticket, int> // CRITICAL: TId is int
    {
        // Any specific methods for Ticket beyond generic ones
        Task<Ticket> GetTicketByCodeAsync(string code);
    }
}