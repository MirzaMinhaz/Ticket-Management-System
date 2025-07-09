// TMS.Application/Services/TicketCounterService.cs
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs; // Ensure these DTOs are defined: TicketCounterDto, CreateTicketCounterDto, UpdateTicketCounterDto
using TMS.Application.Exceptions;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Might need for includes, or if FindAsync/FindSingleAsync use it internally

namespace TMS.Application.Services
{
    public class TicketCounterService : ITicketCounterService
    {
        private readonly ITicketCounterRepository _ticketCounterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TicketCounterService(ITicketCounterRepository ticketCounterRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _ticketCounterRepository = ticketCounterRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketCounterDto>> GetAllTicketCountersAsync()
        {
            var ticketCounters = await _ticketCounterRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TicketCounterDto>>(ticketCounters);
        }

        public async Task<TicketCounterDto> GetTicketCounterByIdAsync(int id)
        {
            var ticketCounter = await _ticketCounterRepository.GetByIdAsync(id);
            if (ticketCounter == null)
            {
                throw new NotFoundException($"Ticket Counter with ID {id} not found.");
            }
            return _mapper.Map<TicketCounterDto>(ticketCounter);
        }

        public async Task<TicketCounterDto> GetTicketCounterByCodeAsync(string counterCode)
        {
            var ticketCounter = await _ticketCounterRepository.FindSingleAsync(tc => tc.CounterCode == counterCode);
            if (ticketCounter == null)
            {
                throw new NotFoundException($"Ticket Counter with code '{counterCode}' not found.");
            }
            return _mapper.Map<TicketCounterDto>(ticketCounter);
        }

        // <<<--- NEW METHOD IMPLEMENTATION
        public async Task<IEnumerable<TicketCounterDto>> GetTicketCountersByLocationAsync(int locationId)
        {
            var ticketCounters = await _ticketCounterRepository.FindAsync(tc => tc.LocationId == locationId);
            return _mapper.Map<IEnumerable<TicketCounterDto>>(ticketCounters);
        }

        public async Task<TicketCounterDto> CreateTicketCounterAsync(CreateTicketCounterDto createDto)
        {
            var existingCounterByName = await _ticketCounterRepository.FindSingleAsync(tc =>
                tc.CounterName == createDto.CounterName && tc.LocationId == createDto.LocationId);
            if (existingCounterByName != null)
            {
                throw new ApplicationException($"A ticket counter with the name '{createDto.CounterName}' already exists at this location.");
            }

            string newCounterCode = await GenerateNextCounterCode();

            var ticketCounter = _mapper.Map<TicketCounter>(createDto);
            ticketCounter.CounterCode = newCounterCode;

            await _ticketCounterRepository.AddAsync(ticketCounter);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<TicketCounterDto>(ticketCounter);
        }

        public async Task UpdateTicketCounterAsync(int id, UpdateTicketCounterDto updateDto)
        {
            var existingTicketCounter = await _ticketCounterRepository.GetByIdAsync(id);
            if (existingTicketCounter == null)
            {
                throw new NotFoundException($"Ticket Counter with ID {id} not found.");
            }

            var duplicateCounter = await _ticketCounterRepository.FindSingleAsync(tc =>
                tc.CounterName == updateDto.CounterName &&
                tc.LocationId == updateDto.LocationId &&
                tc.Id != id);
            if (duplicateCounter != null)
            {
                throw new ApplicationException($"A ticket counter with the name '{updateDto.CounterName}' already exists at this location.");
            }

            _mapper.Map(updateDto, existingTicketCounter);

            _ticketCounterRepository.Update(existingTicketCounter);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteTicketCounterAsync(int id)
        {
            var ticketCounter = await _ticketCounterRepository.GetByIdAsync(id);
            if (ticketCounter == null)
            {
                throw new NotFoundException($"Ticket Counter with ID {id} not found.");
            }

            await _ticketCounterRepository.DeleteAsync(ticketCounter); // <<<--- Using DeleteAsync
            await _unitOfWork.CompleteAsync();
        }

        private async Task<string> GenerateNextCounterCode()
        {
            string lastCode = null;
            var allCounters = await _ticketCounterRepository.GetAllAsync();
            if (allCounters != null && allCounters.Any())
            {
                lastCode = allCounters.OrderByDescending(tc => tc.CounterCode).FirstOrDefault()?.CounterCode;
            }

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastCode) && lastCode.StartsWith("TCO-"))
            {
                if (int.TryParse(lastCode.Substring(4), out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"TCO-{nextNumber:D3}";
        }
    }
}