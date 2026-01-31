// TMS.Domain/Entities/Route.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Route : BaseEntity<int> // Inherit from BaseEntity<int>
    {
        // Primary key is 'Id' from BaseEntity<int>

        public string DepartureLocationCode { get; set; } // FK to Location (int)
        public string DestinationLocationCode { get; set; } // FK to Location (int)
        public string RouteName { get; set; }
        public decimal EstimatedDurationHours { get; set; }
        public string RouteCode { get; set; } // e.g., RTE-0001, RTE-0010

        // Navigation properties
        public Location DepartureLocation { get; set; } // No change here, just type of its PK
        public Location DestinationLocation { get; set; } // No change here, just type of its PK
        public ICollection<Schedule> Schedules { get; set; } = new HashSet<Schedule>();

        public Route()
        {
            Schedules = new HashSet<Schedule>();
        }
    }
}