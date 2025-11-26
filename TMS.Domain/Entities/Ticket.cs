// TMS.Domain/Entities/Ticket.cs
using System;

namespace TMS.Domain.Entities
{
    public class Ticket : BaseEntity<int> // CRITICAL: Ticket ID is int
    {
        // Primary key is 'Id' from BaseEntity<int>

        // Assuming these are INT foreign keys to other tables' INT Primary Keys
        public int UserId { get; set; } // Assuming FK to Users.Id
        public int ScheduleId { get; set; } // Assuming FK to Schedules.Id
        //public int SeatId { get; set; } // Assuming FK to Seats.Id
        public string SeatCode { get; set; }
        public string SeatNumber { get; set; }
        public int BookingCounterId { get; set; } // CRITICAL: FK to TicketCounter.Id (int)
        public int DepartureCounterId { get; set; } // CRITICAL: FK to TicketCounter.Id (int)
        public int ArrivalCounterId { get; set; } // CRITICAL: FK to TicketCounter.Id (int)

        // Navigation properties (if configured)
        // public User User { get; set; }
        // public Schedule Schedule { get; set; }
        // public Seat Seat { get; set; }
        // public TicketCounter BookingCounter { get; set; }
        // public TicketCounter DepartureCounter { get; set; }
        // public TicketCounter ArrivalCounter { get; set; }


        public string TicketCode { get; set; } // String, e.g., TKT-001
        public string PassengerName { get; set; }
        public string PassengerContact { get; set; }
        public decimal FarePaid { get; set; }
        public DateTime BookingDateTime { get; set; }
        public string Status { get; set; }
        // Inherited CreatedAt, LastModifiedAt, CreatedBy, LastModifiedBy
        // Navigation properties
        public User User { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // This is the property you need to add to resolve the error
        public Schedule Schedule { get; set; }

        //public Seat BookedSeat { get; set; }
        public TicketCounter BookingCounter { get; set; }
        public TicketCounter DepartureCounter { get; set; }
        public TicketCounter ArrivalCounter { get; set; }
    }
}