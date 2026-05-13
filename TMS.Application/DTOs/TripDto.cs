// TMS.Application.DTOs/TripDto.cs
namespace TMS.Application.DTOs
{
    public class TripDto
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public DateTime TripDate { get; set; }
        public string Status { get; set; }
        public int AvailableSeats { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }
    }

    public class CreateTripDto
    {
        public int ScheduleId { get; set; }
        public DateTime TripDate { get; set; }   // date-only portion matters
    }
}