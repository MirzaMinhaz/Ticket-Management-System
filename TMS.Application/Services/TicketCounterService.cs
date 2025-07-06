using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class TicketCounterService : ITicketCounterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TicketCounterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketCounterDto>> GetAllTicketCountersAsync()
        {
            // Include related Location data
            var ticketCounters = await _unitOfWork.TicketCounters.GetAllAsync();
            // To include Location, we'd typically need a specific repository method or use Include()
            // For now, let's assume DTO mapping handles basic properties.
            // If you need Location details, you might need a custom GetAllWithLocation() method in repository
            // or perform a projection in the service.
            return _mapper.Map<IEnumerable<TicketCounterDto>>(ticketCounters);
        }

        public async Task<TicketCounterDto> GetTicketCounterByIdAsync(Guid ticketCounterId)
        {
            var ticketCounter = await _unitOfWork.TicketCounters.GetByIdAsync(ticketCounterId);
            return _mapper.Map<TicketCounterDto>(ticketCounter);
        }

        public async Task<TicketCounterDto> CreateTicketCounterAsync(CreateTicketCounterDto ticketCounterDto)
        {
            var ticketCounter = _mapper.Map<TicketCounter>(ticketCounterDto);
            var newTicketCounter = await _unitOfWork.TicketCounters.AddAsync(ticketCounter);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<TicketCounterDto>(newTicketCounter);
        }

        public async Task UpdateTicketCounterAsync(Guid ticketCounterId, UpdateTicketCounterDto ticketCounterDto)
        {
            var existingTicketCounter = await _unitOfWork.TicketCounters.GetByIdAsync(ticketCounterId);
            if (existingTicketCounter == null)
            {
                throw new ApplicationException($"Ticket Counter with ID {ticketCounterId} not found.");
            }

            _mapper.Map(ticketCounterDto, existingTicketCounter);
            existingTicketCounter.LastModifiedAt = DateTime.UtcNow;

            await _unitOfWork.TicketCounters.UpdateAsync(existingTicketCounter);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteTicketCounterAsync(Guid ticketCounterId)
        {
            var ticketCounterToDelete = await _unitOfWork.TicketCounters.GetByIdAsync(ticketCounterId);
            if (ticketCounterToDelete == null)
            {
                throw new ApplicationException($"Ticket Counter with ID {ticketCounterId} not found.");
            }

            await _unitOfWork.TicketCounters.DeleteAsync(ticketCounterToDelete);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<TicketCounterDto>> GetTicketCountersByLocationAsync(Guid locationId)
        {
            var counters = await _unitOfWork.TicketCounters.GetWhereAsync(tc => tc.LocationId == locationId);
            return _mapper.Map<IEnumerable<TicketCounterDto>>(counters);
        }
    }
}