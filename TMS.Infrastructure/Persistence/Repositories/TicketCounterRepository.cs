// TMS.Infrastructure/Persistence/Repositories/TicketCounterRepository.cs
using TMS.Domain.Entities;
using TMS.Application.Interfaces.Persistence;
using TMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Added to use .Where()

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class TicketCounterRepository : GenericRepository<TicketCounter, int>, ITicketCounterRepository
    {
        private readonly TicketManagementDbContext _dbContext;

        public TicketCounterRepository(TicketManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves a list of ticket counters for a specific location.
        /// </summary>
        /// <param name="locationId">The ID of the location to filter by.</param>
        /// <returns>A list of TicketCounter entities.</returns>
        public async Task<IEnumerable<TicketCounter>> GetTicketCountersByLocationAsync(int locationId)
        {
            return await _dbContext.TicketCounters
                .Where(tc => tc.LocationId == locationId)
                .Include(tc => tc.Location) // Include the Location navigation property if needed for a richer result
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a single ticket counter by its unique counter code.
        /// </summary>
        /// <param name="counterCode">The unique code of the ticket counter.</param>
        /// <returns>The TicketCounter entity if found, otherwise null.</returns>
        public async Task<TicketCounter> GetTicketCounterByCodeAsync(string counterCode)
        {
            // Use FirstOrDefaultAsync to get a single entity or null if not found
            return await _dbContext.TicketCounters
                .FirstOrDefaultAsync(tc => tc.CounterCode == counterCode);
        }

        // You may want to add more specific methods here as your application grows,
        // for example, to get counters with their related booking, departure, or arrival tickets.
        // public async Task<TicketCounter> GetTicketCounterWithTicketsAsync(int counterId)
        // {
        //     return await _dbContext.TicketCounters
        //         .Include(tc => tc.BookingTickets)
        //         .Include(tc => tc.DepartureTickets)
        //         .Include(tc => tc.ArrivalTickets)
        //         .FirstOrDefaultAsync(tc => tc.Id == counterId);
        // }
    }
}
