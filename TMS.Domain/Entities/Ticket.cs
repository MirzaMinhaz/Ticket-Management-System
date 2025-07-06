// TMS.Domain/Entities/Ticket.cs
using System;
using System.Collections.Generic; // Make sure this is present for ICollection

namespace TMS.Domain.Entities
{
    public class Ticket : BaseEntity
    {
        // EF Core will map 'Id' from BaseEntity to TicketId column via configuration

        public Guid UserId { get; set; } // Foreign key to User
        public Guid ScheduleId { get; set; } // Foreign key to Schedule
        public Guid BookedSeatId { get; set; } // Foreign key to Seat

        // NEW Foreign Keys for Ticket Counters (Nullable as per design)
        public Guid? BookingCounterId { get; set; }
        public Guid? DepartureCounterId { get; set; }
        public Guid? ArrivalCounterId { get; set; }

        public string PassengerName { get; set; }
        public string PassengerContact { get; set; } // Email or Phone
        public decimal FarePaid { get; set; }
        public DateTime BookingDateTime { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } // 'Confirmed', 'Cancelled', 'Pending Payment'

        // Navigation properties (Ensure these classes exist in TMS.Domain.Entities)
        public User User { get; set; } // CS0246 for User
        public Schedule Schedule { get; set; } // CS0246 for Schedule
        public Seat BookedSeat { get; set; } // CS0246 for Seat

        // Navigation properties for TicketCounters (if you added these)
        public TicketCounter BookingCounter { get; set; }
        public TicketCounter DepartureCounter { get; set; }
        public TicketCounter ArrivalCounter { get; set; }

        public ICollection<Comment> Comments { get; set; } // CS0246 for Comment
    }
}