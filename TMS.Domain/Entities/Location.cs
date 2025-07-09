// TMS.Domain/Entities/Location.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Location : BaseEntity<int> // Inherit from BaseEntity<int>
    {
        // Primary key is 'Id' from BaseEntity<int>

        public string Name { get; set; }
        public string Type { get; set; } // e.g., City, Bus Terminal, Train Station
        public string Address { get; set; }
        public string LocationCode { get; set; } // e.g., LOC-0001, LOC-0010

        // Navigation properties
        public ICollection<TicketCounter> TicketCounters { get; set; } = new HashSet<TicketCounter>();
        public ICollection<Route> DepartureRoutes { get; set; } = new HashSet<Route>(); // Routes where this is the departure
        public ICollection<Route> DestinationRoutes { get; set; } = new HashSet<Route>(); // Routes where this is the destination

        // Constructor to ensure collections are initialized
        public Location()
        {
            TicketCounters = new HashSet<TicketCounter>();
            DepartureRoutes = new HashSet<Route>();
            DestinationRoutes = new HashSet<Route>();
        }
    }
}