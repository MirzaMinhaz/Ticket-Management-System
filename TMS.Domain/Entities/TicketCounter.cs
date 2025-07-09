// In TicketCounter.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class TicketCounter : BaseEntity<int> // Change from BaseEntity to BaseEntity<int>
    {
        // TicketCounterId will now be the 'Id' from BaseEntity<int>

        public int LocationId { get; set; } // Change from Guid to int (Foreign key to Location)

        public string CounterName { get; set; }
        public string CounterCode { get; set; } // This will store TCO-001, TCO-002 etc.
        public string AddressDetails { get; set; }
        public string ContactNumber { get; set; }
        public string OperatingHours { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Location Location { get; set; } // Navigation property to parent Location
        public ICollection<Ticket> BookingTickets { get; set; } // Tickets booked at this counter
        public ICollection<Ticket> DepartureTickets { get; set; } // Tickets departing from this counter
        public ICollection<Ticket> ArrivalTickets { get; set; } // Tickets arriving at this counter

        // Constructor to initialize collections (good practice)
        public TicketCounter()
        {
            BookingTickets = new HashSet<Ticket>();
            DepartureTickets = new HashSet<Ticket>();
            ArrivalTickets = new HashSet<Ticket>();
        }
    }
}