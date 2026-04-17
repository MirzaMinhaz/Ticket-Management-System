using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces.Persistence
{
    public interface IOperatorRepository
    {
        Task<List<Operator>> GetAllAsync();
        Task<Operator> GetByIdAsync(int id);
        Task<Operator?> GetByNameAsync(string name);
        Task<Operator> AddAsync(Operator entity);
        Task UpdateAsync(Operator entity);
        Task DeleteAsync(int id);
    }
}

