using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TMS.Application.Interfaces.Persistence;
using TMS.Domain.Entities;

namespace TMS.Infrastructure.Persistence.Repositories
{
    public class OperatorRepository : IOperatorRepository
    {
        private readonly TicketManagementDbContext _context;

        public OperatorRepository(TicketManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<Operator>> GetAllAsync()
        {
            return await _context.Operators.ToListAsync();
        }

        public async Task<Operator> GetByIdAsync(int id)
        {
            return await _context.Operators.FindAsync(id);
        }

        public async Task<Operator> AddAsync(Operator entity)
        {
            
            try
            {
                _context.Operators.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ EF Core Save Error: {ex.Message}");
                Console.WriteLine($"🔍 Inner Exception: {ex.InnerException?.Message}");
                throw; // rethrow to bubble up to controller
            }
        }

        public async Task UpdateAsync(Operator entity)
        {
            _context.Operators.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var op = await _context.Operators.FindAsync(id);
            if (op != null)
            {
                _context.Operators.Remove(op);
                await _context.SaveChangesAsync();
            }
        }
    }
}

