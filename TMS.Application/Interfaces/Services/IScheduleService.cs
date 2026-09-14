using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs.Schedule;

namespace TMS.Application.Interfaces.Services
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
        Task<ScheduleDto> GetScheduleByIdAsync(int id);
        Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto createDto);
        Task UpdateScheduleAsync(int id, UpdateScheduleDto updateDto);
        Task<ScheduleDto?> UpdateBaseFareAsync(int scheduleId, decimal baseFare); // ← add this line
        Task DeleteScheduleAsync(int id);
    }
}