using System;
using System.ComponentModel.DataAnnotations.Schema; // For [Column]

namespace TMS.Domain.Entities
{
    public class Location : BaseEntity 
    {


        public string Name { get; set; }
        public string Type { get; set; } // e.g., 'City', 'Terminal', 'Bus Stop'
        public string Address { get; set; }
        //public decimal? Latitude { get; set; } // Nullable, as per schema design
        //public decimal? Longitude { get; set; } // Nullable, as per schema design

        // Navigation property for TicketCounters at this location
        public ICollection<TicketCounter> TicketCounters { get; set; }
    }
}