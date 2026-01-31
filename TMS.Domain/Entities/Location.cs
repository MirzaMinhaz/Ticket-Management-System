// TMS.Domain/Entities/Location.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Location : BaseEntity<int>
    {
        // Existing properties (Id, LocationCode, Name, etc.)
        public string LocationCode { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }

        public bool IsActive { get; set; }

        // Navigation properties for relationships
        // Collection of TicketCounters at this location
        public ICollection<TicketCounter> TicketCounters { get; set; } = new List<TicketCounter>();

        // Collection of Routes where this location is the departure point
        public ICollection<Route> DepartureRoutes { get; set; } = new List<Route>();

        // Collection of Routes where this location is the destination point
        public ICollection<Route> DestinationRoutes { get; set; } = new List<Route>();
    }
}