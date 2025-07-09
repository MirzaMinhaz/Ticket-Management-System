// TMS.Domain/Entities/Ticket.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Ticket : BaseEntity<int> // Inherit from BaseEntity<int>
    {
        // Primary key is 'Id' from BaseEntity<int>

        public int UserId { get; set; }      // Foreign key to User (int)
        public int ScheduleId { get; set; }  // Foreign key to Schedule (int)
        public int SeatId { get; set; }      // Foreign key to Seat (int)

        // Foreign Keys for Ticket Counters (Nullable if optional)
        public int? BookingCounterId { get; set; }
        public int? DepartureCounterId { get; set; }
        public int? ArrivalCounterId { get; set; }

        public string TicketCode { get; set; } // e.g., TIC-0001, TIC-0010
        public string PassengerName { get; set; }
        public string PassengerContact { get; set; } // Email or Phone
        public decimal FarePaid { get; set; }
        public DateTime BookingDateTime { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } // 'Confirmed', 'Cancelled', 'Pending Payment', etc.

        // Navigation properties
        public User User { get; set; }
        public Schedule Schedule { get; set; }
        public Seat BookedSeat { get; set; } // Renamed from 'Seat' for clarity

        // Navigation properties for TicketCounters
        public TicketCounter BookingCounter { get; set; }
        public TicketCounter DepartureCounter { get; set; }
        public TicketCounter ArrivalCounter { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public Ticket()
        {
            Comments = new List<Comment>();
        }
    }
}