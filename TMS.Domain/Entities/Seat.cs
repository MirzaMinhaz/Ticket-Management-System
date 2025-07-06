// TMS.Domain/Entities/Seat.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Seat : BaseEntity // Inherits Id (SeatId), CreatedAt, etc.
    {
        // Primary key will be Id from BaseEntity, mapped to SeatId column

        public Guid ScheduleId { get; set; } // Foreign key to Schedule
        public string SeatNumber { get; set; } // e.g., 'A1', '12'
        public bool IsBooked { get; set; } = false; // Indicates if currently booked for a specific schedule

        // Navigation property to the Schedule this seat belongs to
        public Schedule Schedule { get; set; }

        // Navigation property: A Seat can be booked by multiple Tickets (over different schedules/times)
        // This makes it the "one" side of a one-to-many relationship with Ticket
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}