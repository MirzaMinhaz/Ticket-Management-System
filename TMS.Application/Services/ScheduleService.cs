using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs.Route;
using TMS.Application.DTOs.Schedule;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Repositories;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFareHistoryRepository _fareHistoryRepository;

        public ScheduleService(
            IScheduleRepository scheduleRepository,
            IFareHistoryRepository fareHistoryRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _scheduleRepository = scheduleRepository;
            _fareHistoryRepository = fareHistoryRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<ScheduleDto> GetScheduleByIdAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto createDto)
        {
            try
            {
                var schedule = new Schedule
                {
                    RouteId = createDto.RouteId,
                    VehicleId = createDto.VehicleId,
                    DepartureDateTime = createDto.DepartureDateTime,
                    ArrivalDateTime = createDto.ArrivalDateTime,
                    BaseFare = createDto.BaseFare,
                    Status = createDto.Status ?? "Scheduled",
                    ScheduleCode = await GenerateUniqueScheduleCode(), // ✅ ensure not null
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "SystemUser",
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = "SystemUser"
                };

                await _scheduleRepository.AddAsync(schedule);
                await _unitOfWork.CompleteAsync();

                return new ScheduleDto
                {
                    Id = schedule.Id,
                    RouteId = schedule.RouteId,
                    VehicleId = schedule.VehicleId,
                    DepartureDateTime = schedule.DepartureDateTime,
                    ArrivalDateTime = schedule.ArrivalDateTime,
                    BaseFare = schedule.BaseFare,
                    Status = schedule.Status,
                    ScheduleCode = schedule.ScheduleCode,
                    CreatedAt = schedule.CreatedAt,
                    CreatedBy = schedule.CreatedBy,
                    LastModifiedAt = schedule.LastModifiedAt,
                    LastModifiedBy = schedule.LastModifiedBy
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[CreateScheduleAsync] Error: {ex.Message}");
                throw new ApplicationException("Unexpected error occurred while creating schedule.", ex);
            }
        }




        public async Task UpdateScheduleAsync(int id, UpdateScheduleDto updateDto)
        {
            var existingSchedule = await _scheduleRepository.GetByIdAsync(id);
            if (existingSchedule == null) return;

            _mapper.Map(updateDto, existingSchedule);
            await _scheduleRepository.UpdateAsync(existingSchedule);
        }

        public async Task<ScheduleDto?> UpdateBaseFareAsync(int scheduleId, decimal baseFare)
        {
            if (baseFare < 0 || baseFare >= 5000)
                throw new ApplicationException("Fare must be between 0 and 4999.");

            var schedule = await _scheduleRepository.GetByIdWithDetailsAsync(scheduleId);
            if (schedule == null) return null;

            var oldFare = schedule.BaseFare;

            if (oldFare != baseFare)
            {
                await _fareHistoryRepository.AddAsync(new FareHistory
                {
                    ScheduleId = schedule.Id,
                    OperatorName = schedule.Vehicle.Operator.Name,
                    VehicleName = schedule.Vehicle.Model ?? schedule.Vehicle.VehicleCode,
                    RouteCode = $"{schedule.Route.DepartureLocationCode}-{schedule.Route.DestinationLocationCode}",
                    PreviousFare = oldFare,
                    CurrentFare = baseFare,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = "System"
                });
            }

            schedule.BaseFare = baseFare;
            schedule.LastModifiedAt = DateTime.UtcNow;
            schedule.LastModifiedBy = "System";

            await _scheduleRepository.UpdateAsync(schedule);
            await _unitOfWork.CompleteAsync();   // schedule + history together

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<IEnumerable<FareHistoryDto>> GetRecentFareHistoryAsync(int take = 5)
        {
            take = Math.Clamp(take, 1, 50);
            var rows = await _fareHistoryRepository.GetRecentAsync(take);

            return rows.Select(x => new FareHistoryDto
            {
                Id = x.Id,
                OperatorName = x.OperatorName,
                VehicleName = x.VehicleName,
                PreviousFare = x.PreviousFare,
                CurrentFare = x.CurrentFare,
                CreatedAt = x.CreatedAt
            }).ToList();
        }

        public async Task DeleteScheduleAsync(int id)
        {
            await _scheduleRepository.DeleteAsync(id);
        }

        private async Task<string> GenerateUniqueScheduleCode()
        {
            var allSchedules = await _scheduleRepository.GetAllAsync();

            string lastCode = allSchedules
                                .Select(s => s.ScheduleCode)
                                .Where(code => !string.IsNullOrEmpty(code) && code.StartsWith("SCH-"))
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

            return $"SCH-{nextNumber:D3}"; // Formats as SCH-001, SCH-002, etc.
        }



    }
}
