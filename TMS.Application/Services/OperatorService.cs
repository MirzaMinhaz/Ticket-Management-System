using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Application.DTOs.Operator;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class OperatorService : IOperatorService
    {
        private readonly IOperatorRepository _repository;

        public OperatorService(IOperatorRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<OperatorDto>> GetAllAsync()
        {
            var operators = await _repository.GetAllAsync();
            return operators.Select(o => new OperatorDto
            {
                Id = o.Id,
                Name = o.Name,
                Type = o.Type,
                OperatorCode = o.OperatorCode,
                CreatedAt = o.CreatedAt
            }).ToList();
        }

        public async Task<OperatorDto> GetByIdAsync(int id)
        {
            var op = await _repository.GetByIdAsync(id);
            if (op == null) throw new Exception($"Operator with ID {id} not found.");

            return new OperatorDto
            {
                Id = op.Id,
                Name = op.Name,
                Type = op.Type,
                OperatorCode = op.OperatorCode,
                CreatedAt = op.CreatedAt
            };
        }

        public async Task<OperatorDto> CreateAsync(CreateOperatorDto dto)
        {
            var operatorCode = await GenerateOperatorCodeAsync();

            var entity = new Operator
            {
                Name = dto.Name,
                Type = dto.Type,
                OperatorCode = operatorCode,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _repository.AddAsync(entity);

            return new OperatorDto
            {
                Id = result.Id,
                Name = result.Name,
                Type = result.Type,
                OperatorCode = result.OperatorCode,
                CreatedAt = result.CreatedAt
            };
        }


        public async Task UpdateAsync(int id, UpdateOperatorDto dto)
        {
            var op = await _repository.GetByIdAsync(id);
            if (op == null) throw new Exception($"Operator with ID {id} not found.");

            op.Name = dto.Name;
            op.Type = dto.Type;
            op.OperatorCode = dto.OperatorCode;
            op.LastModifiedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(op);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        private async Task<string> GenerateOperatorCodeAsync()
        {
            var allOperators = await _repository.GetAllAsync();
            var nextId = allOperators.Any() ? allOperators.Max(o => o.Id) + 1 : 1;
            return $"OPT-{nextId:D4}";
        }

    }
}

