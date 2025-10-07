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
                CreatedAt = o.CreatedAt ?? DateTime.MinValue
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
                CreatedAt = op.CreatedAt ?? DateTime.MinValue
            };
        }

        public async Task<OperatorDto> CreateAsync(CreateOperatorDto dto)
        {
            try
            {
                var operatorCode = await GenerateOperatorCodeAsync();

                var entity = new Operator
                {
                    Name = dto.Name,
                    Type = dto.Type,
                    OperatorCode = operatorCode,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedBy = "System"
                };

                var result = await _repository.AddAsync(entity);

                return new OperatorDto
                {
                    Id = result.Id,
                    Name = result.Name,
                    Type = result.Type,
                    OperatorCode = result.OperatorCode,
                    CreatedAt = result.CreatedAt ?? DateTime.MinValue
                };
            }
            catch (Exception ex)
            {
                // You can log the error here using your preferred logging framework
                Console.WriteLine($"Error in CreateAsync: {ex.Message}");
                throw; // Optionally rethrow to let the caller handle it
            }
        }



        public async Task UpdateAsync(int id, UpdateOperatorDto dto)
        {
            var op = await _repository.GetByIdAsync(id);
            if (op == null) throw new Exception($"Operator with ID {id} not found.");

            op.Name = dto.Name;
            op.Type = dto.Type;

            await _repository.UpdateAsync(op);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        //private async Task<string> GenerateOperatorCodeAsync()
        //{
        //    var allOperators = await _repository.GetAllAsync();
        //    var nextId = allOperators.Any() ? allOperators.Max(o => o.Id) + 1 : 1;
        //    return $"OPT-{nextId:D4}";
        //}
        //private async Task<string> GenerateOperatorCodeAsync()
        //{
        //    var lastOperator = await _repository
        //        .GetAllAsync();

        //    var lastCode = lastOperator
        //        .OrderByDescending(o => o.OperatorCode)
        //        .FirstOrDefault()?.OperatorCode;

        //    if (string.IsNullOrEmpty(lastCode))
        //        return "OPT-001";

        //    var numberPart = int.Parse(lastCode.Split('-')[1]);
        //    return $"OPT-{(numberPart + 1):D3}";
        //}
        private async Task<string> GenerateOperatorCodeAsync()
        {
            var allOperators = await _repository.GetAllAsync();

            if (!allOperators.Any())
                return "OPT-001";

            // Extract numeric part safely and sort by that
            var maxNumber = allOperators
                .Select(o =>
                {
                    if (string.IsNullOrEmpty(o.OperatorCode))
                        return 0;

                    var parts = o.OperatorCode.Split('-');
                    return parts.Length == 2 && int.TryParse(parts[1], out var n) ? n : 0;
                })
                .Max();

            return $"OPT-{(maxNumber + 1):D3}";
        }


    }
}

