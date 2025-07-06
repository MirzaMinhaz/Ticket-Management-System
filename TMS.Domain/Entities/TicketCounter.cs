using System;
using System.ComponentModel.DataAnnotations.Schema; // For [Column]
using System.Collections.Generic;
using System.Net.Sockets;

namespace TMS.Domain.Entities
{
    public class TicketCounter : BaseEntity // Inherits BaseEntity for common properties
    {
        // EF Core will map 'Id' from BaseEntity to TicketCounterId column via configuration
        public Guid LocationId { get; set; } // Foreign key to Location

        public string CounterName { get; set; }
        public string CounterCode { get; set; }
        public string AddressDetails { get; set; }
        public string ContactNumber { get; set; }
        public string OperatingHours { get; set; }
        //public decimal? Latitude { get; set; } // Specific to counter
        //public decimal? Longitude { get; set; } // Specific to counter
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Location Location { get; set; } // Navigation property to parent Location
        public ICollection<Ticket> BookingTickets { get; set; } // Tickets booked at this counter
        public ICollection<Ticket> DepartureTickets { get; set; } // Tickets departing from this counter
        public ICollection<Ticket> ArrivalTickets { get; set; } // Tickets arriving at this counter
    }
}