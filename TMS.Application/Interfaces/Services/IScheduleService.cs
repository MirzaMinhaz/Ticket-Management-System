// TMS.Application/Interfaces/Services/IScheduleService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS.Application.DTOs.Schedule;

namespace TMS.Application.Interfaces.Services
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
        Task<ScheduleDto> GetScheduleByIdAsync(int id); // CRITICAL: int ID
        Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto createDto);
        Task UpdateScheduleAsync(int id, UpdateScheduleDto updateDto); // CRITICAL: int ID
        Task DeleteScheduleAsync(int id); // CRITICAL: int ID
    }
}
