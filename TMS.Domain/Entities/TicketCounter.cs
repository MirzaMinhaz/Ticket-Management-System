// TMS.Domain/Entities/TicketCounter.cs

using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class TicketCounter : BaseEntity<int>
    {
        // Existing properties
        public int LocationId { get; set; }
        public string CounterName { get; set; }
        public string CounterCode { get; set; }
        public string AddressDetails { get; set; }
        public string ContactNumber { get; set; }
        public string OperatingHours { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public Location Location { get; set; }

        // Add these three properties to resolve the error
        public ICollection<Ticket> BookingTickets { get; set; } = new List<Ticket>();
        public ICollection<Ticket> DepartureTickets { get; set; } = new List<Ticket>();
        public ICollection<Ticket> ArrivalTickets { get; set; } = new List<Ticket>();
    }
}