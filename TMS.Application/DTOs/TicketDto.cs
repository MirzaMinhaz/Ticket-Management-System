// TMS.Application.DTOs/Ticket/TicketDto.cs
namespace TMS.Application.DTOs.Ticket
{
    public class TicketDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }           // ← was ScheduleId
        public string TicketCode { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerContact { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public string? SeatCode { get; set; }
        public decimal FarePaid { get; set; }
        public DateTime BookingDateTime { get; set; }
        public int BookingCounterId { get; set; }
        public int DepartureCounterId { get; set; }
        public int ArrivalCounterId { get; set; }
        public string Status { get; set; } = string.Empty;   // Booked / Cancelled
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class CreateTicketDto
    {
        public int TripId { get; set; }           // ← was ScheduleId
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerContact { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public string? SeatCode { get; set; }
        public decimal FarePaid { get; set; }
        public DateTime BookingDateTime { get; set; }
        public int BookingCounterId { get; set; }
        public int DepartureCounterId { get; set; }
        public int ArrivalCounterId { get; set; }
    }

    public class UpdateTicketDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }           // ← was ScheduleId
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerContact { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public string? SeatCode { get; set; }
        public decimal FarePaid { get; set; }
        public DateTime BookingDateTime { get; set; }
        public int BookingCounterId { get; set; }
        public int DepartureCounterId { get; set; }
        public int ArrivalCounterId { get; set; }
    }

    public class CancelTicketDto
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}