// TMS.Application.Interfaces/ITripService.cs
using TMS.Application.DTOs;

namespace TMS.Application.Interfaces
{
    public interface ITripService
    {
        /// <summary>
        /// Returns the existing Trip for this schedule+date, or creates one if none exists.
        /// This is the "find-or-create" entry point called before every ticket booking.
        /// </summary>
        Task<TripDto> FindOrCreateTripAsync(int scheduleId, DateTime tripDate);
        Task<TripDto?> GetByIdAsync(int id);
        Task<IEnumerable<TripDto>> GetAllAsync();
        Task<IEnumerable<string>> GetBookedSeatNumbersAsync(int tripId);
    }
}