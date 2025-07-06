// TMS.Domain/Entities/Seat.cs
using System;

namespace TMS.Domain.Entities
{
    public class Seat : BaseEntity
    {
        public Guid ScheduleId { get; set; } // FK to Schedule
        public string SeatNumber { get; set; } // e.g., 'A1', '12'
        public bool IsBooked { get; set; } = false;
        public Guid? TicketId { get; set; } // FK to Ticket (nullable, unique if booked)

        // Navigation properties
        public Schedule Schedule { get; set; }
        public Ticket Ticket { get; set; } // Nullable, one-to-one with Ticket if booked
    }
}