using AutoMapper;
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
        

        public ScheduleService(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _scheduleRepository = scheduleRepository;
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
