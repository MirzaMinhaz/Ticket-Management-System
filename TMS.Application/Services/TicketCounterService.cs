// TMS.Application/Services/TicketCounterService.cs
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Application.DTOs;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class TicketCounterService : ITicketCounterService
    {
        private readonly ITicketCounterRepository _ticketCounterRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TicketCounterService(ITicketCounterRepository ticketCounterRepository, ILocationRepository locationRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _ticketCounterRepository = ticketCounterRepository;
            _locationRepository = locationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketCounterDto>> GetAllTicketCountersAsync()
        {
            var ticketCounters = await _ticketCounterRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TicketCounterDto>>(ticketCounters);
        }

        public async Task<TicketCounterDto> GetTicketCounterByIdAsync(int id) // CRITICAL: int ID
        {
            var ticketCounter = await _ticketCounterRepository.GetByIdAsync(id);
            return _mapper.Map<TicketCounterDto>(ticketCounter);
        }

        public async Task<IEnumerable<TicketCounterDto>> GetTicketCountersByLocationAsync(string locationCode)
        {
            // You will need to implement a repository method to get ticket counters by location ID.
            var ticketCounters = await _ticketCounterRepository.GetTicketCountersByLocationAsync(locationCode);
            return _mapper.Map<IEnumerable<TicketCounterDto>>(ticketCounters);
        }

        public async Task<TicketCounterDto> GetTicketCounterByCodeAsync(string counterCode)
        {
            var ticketCounter = await _ticketCounterRepository.GetTicketCounterByCodeAsync(counterCode);
            return _mapper.Map<TicketCounterDto>(ticketCounter);
        }

        public async Task<TicketCounterDto> CreateTicketCounterAsync(CreateTicketCounterDto createDto)
        {
            try
            {
                // Validate LocationCode exists
                var locationExists = await _locationRepository.GetByCodeAsync(createDto.LocationCode);
                if (locationExists == null)
                {
                    throw new ArgumentException($"Location with code '{createDto.LocationCode}' not found.");
                }

                var ticketCounter = _mapper.Map<TicketCounter>(createDto);

                ticketCounter.CounterCode = await GenerateUniqueTicketCounterCode();
                ticketCounter.IsActive = true;
                ticketCounter.OperatingHours = "7:00 AM - 11:00PM";

                ticketCounter.CreatedAt = DateTime.UtcNow;
                ticketCounter.CreatedBy = "SystemUser";
                ticketCounter.LastModifiedAt = DateTime.UtcNow;
                ticketCounter.LastModifiedBy = "SystemUser";

                await _ticketCounterRepository.AddAsync(ticketCounter);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<TicketCounterDto>(ticketCounter);
            }
            catch (Exception ex)
            {
                // Optional: log the error to a logging service
                Console.Error.WriteLine($"[CreateTicketCounterAsync] Error: {ex.Message}");

                // Rethrow or wrap in a custom exception if needed
                throw new ApplicationException("Failed to create ticket counter.", ex);
            }
        }


        // TMS.Application/Services/TicketCounterService.cs
        public async Task UpdateTicketCounterAsync(int id, UpdateTicketCounterDto updateDto) // CRITICAL: int ID
        {
            var existingTicketCounter = await _ticketCounterRepository.GetByIdAsync(id);
            if (existingTicketCounter == null)
            {
                throw new Exception($"Ticket Counter with ID {id} not found.");
            }

            // Validate LocationCode (FK, string) exists if it's being updated
            if (existingTicketCounter.LocationCode != updateDto.LocationCode)
            {
                // Use GetByCodeAsync since LocationCode is a string
                var locationExists = await _locationRepository.GetByCodeAsync(updateDto.LocationCode);
                if (locationExists == null)
                {
                    // Update exception message to reference LocationCode
                    throw new Exception($"Location with code {updateDto.LocationCode} not found.");
                }
            }

            _mapper.Map(updateDto, existingTicketCounter);
            existingTicketCounter.LastModifiedAt = DateTime.UtcNow;
            existingTicketCounter.LastModifiedBy = "SystemUser";

            _ticketCounterRepository.Update(existingTicketCounter);
            await _unitOfWork.CompleteAsync();
        }

        //public async Task<TicketCounterDto> CreateTicketCounterAsync(CreateTicketCounterDto createDto)
        //{
        //    // Validate LocationId (FK, int) exists
        //    var locationExists = await _locationRepository.GetByIdAsync(createDto.locationId); // CRITICAL: Pass int ID
        //    if (locationExists == null)
        //    {
        //        throw new Exception($"Location with ID {createDto.locationId} not found.");
        //    }

        //    var ticketCounter = _mapper.Map<TicketCounter>(createDto);

        //    // TicketCounter.Id (int) is auto-incremented by DB. DO NOT SET IT HERE.
        //    // Generate CounterCode (string)
        //    ticketCounter.CounterCode = await GenerateUniqueTicketCounterCode();

        //    ticketCounter.CreatedAt = DateTime.UtcNow;
        //    ticketCounter.CreatedBy = "SystemUser";
        //    ticketCounter.LastModifiedAt = DateTime.UtcNow;
        //    ticketCounter.LastModifiedBy = "SystemUser";

        //    await _ticketCounterRepository.AddAsync(ticketCounter);
        //    await _unitOfWork.CompleteAsync(); // This saves to DB and populates `ticketCounter.Id`

        //    return _mapper.Map<TicketCounterDto>(ticketCounter);
        //}

        //public async Task UpdateTicketCounterAsync(int id, UpdateTicketCounterDto updateDto) // CRITICAL: int ID
        //{
        //    var existingTicketCounter = await _ticketCounterRepository.GetByIdAsync(id);
        //    if (existingTicketCounter == null)
        //    {
        //        throw new Exception($"Ticket Counter with ID {id} not found.");
        //    }

        //    // Validate LocationId (FK, int) exists if it's being updated
        //    if (existingTicketCounter.LocationId != updateDto.locationId)
        //    {
        //        var locationExists = await _locationRepository.GetByIdAsync(updateDto.locationId); // CRITICAL: Pass int ID
        //        if (locationExists == null)
        //        {
        //            throw new Exception($"Location with ID {updateDto.locationId} not found.");
        //        }
        //    }

        //    _mapper.Map(updateDto, existingTicketCounter);
        //    existingTicketCounter.LastModifiedAt = DateTime.UtcNow;
        //    existingTicketCounter.LastModifiedBy = "SystemUser";

        //    _ticketCounterRepository.Update(existingTicketCounter);
        //    await _unitOfWork.CompleteAsync();
        //}

        public async Task DeleteTicketCounterAsync(int id) // CRITICAL: int ID
        {
            var existingTicketCounter = await _ticketCounterRepository.GetByIdAsync(id);
            if (existingTicketCounter == null)
            {
                throw new Exception($"Ticket Counter with ID {id} not found.");
            }
            await _ticketCounterRepository.DeleteAsync(existingTicketCounter);
            await _unitOfWork.CompleteAsync();
        }

        private async Task<string> GenerateUniqueTicketCounterCode()
        {
            var allCounters = await _ticketCounterRepository.GetAllAsync();
            string lastCode = allCounters
                                .Select(tc => tc.CounterCode)
                                .Where(code => code != null && code.StartsWith("TC-"))
                                .OrderByDescending(code => code)
                                .FirstOrDefault();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastHyphenIndex = lastCode.LastIndexOf('-');
                if (lastHyphenIndex != -1 && lastCode.Length > lastHyphenIndex + 1)
                {
                    string numericPart = lastCode.Substring(lastHyphenIndex + 1);
                    if (int.TryParse(numericPart, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }
            }
            return $"TC-{nextNumber:D3}"; // Formats as TC-001, TC-002, etc.
        }
    }
}