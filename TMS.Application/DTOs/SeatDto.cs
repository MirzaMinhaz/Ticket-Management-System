// TMS.Application/DTOs/SeatDto.cs
namespace TMS.Application.DTOs
{
    public class SeatDto
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; }
        public string SeatCode { get; set; }
        public bool IsBooked { get; set; }

        // Computed property
        public string Status => IsBooked ? "reserved" : "available";
    }

}
