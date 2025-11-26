using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using TMS.Domain.Entities;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly TicketManagementDbContext _context;

        public SeatRepository(TicketManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Seat>> GetSeatsByScheduleAsync(int scheduleId)
        {
            return await _context.Seats
                .Where(s => s.ScheduleId == scheduleId)
                .ToListAsync();
        }

        public async Task<Seat> GetSeatByIdAsync(int seatId)
        {
            return await _context.Seats.FindAsync(seatId);
        }

        public async Task UpdateSeatAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
        }

        public async Task AddSeatAsync(Seat seat)
        {
            await _context.Seats.AddAsync(seat);
            await _context.SaveChangesAsync();
        }
    }
}