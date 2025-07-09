// TMS.Domain/Entities/Seat.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Seat : BaseEntity<int> // Inherit from BaseEntity<int>
    {
        // Primary key is 'Id' from BaseEntity<int>

        public int ScheduleId { get; set; } // Foreign key to Schedule (int)
        public string SeatNumber { get; set; } // e.g., 'A1', '12'
        public bool IsBooked { get; set; } = false; // Indicates if currently booked for a specific schedule
        public string SeatCode { get; set; } // e.g., SEA-0001 (This would be per unique seat across all schedules, or per schedule+seatnumber)

        // Navigation property to the Schedule this seat belongs to
        public Schedule Schedule { get; set; }

        // Navigation property: A Seat can be booked by multiple Tickets (over different schedules/times)
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        public Seat()
        {
            Tickets = new List<Ticket>();
        }
    }
}