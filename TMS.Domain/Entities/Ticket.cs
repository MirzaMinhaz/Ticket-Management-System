// TMS.Domain/Entities/Ticket.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Ticket : BaseEntity // Inherits Id (TicketId), CreatedAt, etc.
    {
        // Primary key will be Id from BaseEntity, mapped to TicketId column

        public Guid UserId { get; set; }      // Foreign key to User
        public Guid ScheduleId { get; set; }  // Foreign key to Schedule
        public Guid SeatId { get; set; }      // Foreign key to Seat (the specific seat booked for this ticket)

        // Foreign Keys for Ticket Counters (Nullable if optional)
        public Guid? BookingCounterId { get; set; }
        public Guid? DepartureCounterId { get; set; }
        public Guid? ArrivalCounterId { get; set; }

        public string PassengerName { get; set; }
        public string PassengerContact { get; set; } // Email or Phone
        public decimal FarePaid { get; set; }
        public DateTime BookingDateTime { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } // 'Confirmed', 'Cancelled', 'Pending Payment', etc.

        // Navigation properties
        public User User { get; set; }
        public Schedule Schedule { get; set; }

        // This is the property that caused the confusion and the error:
        // 'BookedSeat' is just a name for the navigation property that points to the 'Seat' entity.
        public Seat BookedSeat { get; set; } // Renamed from 'Seat' to 'BookedSeat' for clarity in Ticket entity

        // Navigation properties for TicketCounters
        public TicketCounter BookingCounter { get; set; }
        public TicketCounter DepartureCounter { get; set; }
        public TicketCounter ArrivalCounter { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}