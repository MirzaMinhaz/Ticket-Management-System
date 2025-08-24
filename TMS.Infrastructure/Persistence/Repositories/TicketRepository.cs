// TMS.Infrastructure/Persistence/Repositories/TicketRepository.cs
using TMS.Domain.Entities;
using TMS.Application.Interfaces.Persistence;
using TMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : GenericRepository<Ticket, int>, ITicketRepository // CRITICAL: int ID
    {
        private readonly TicketManagementDbContext _dbContext;

        public TicketRepository(TicketManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Ticket> GetTicketByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.TicketCode == code);
        }
    }
}