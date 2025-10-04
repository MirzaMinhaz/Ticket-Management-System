using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Application.DTOs.Operator;

namespace TMS.Application.Interfaces.Services
{
    public interface IOperatorService
    {
        Task<List<OperatorDto>> GetAllAsync();
        Task<OperatorDto> GetByIdAsync(int id);
        Task<OperatorDto> CreateAsync(CreateOperatorDto dto);
        Task UpdateAsync(int id, UpdateOperatorDto dto);
        Task DeleteAsync(int id);
    }
}

