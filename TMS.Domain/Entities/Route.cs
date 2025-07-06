// TMS.Domain/Entities/Route.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Route : BaseEntity
    {
        public Guid DepartureLocationId { get; set; } // FK to Location
        public Guid DestinationLocationId { get; set; } // FK to Location
        public string RouteName { get; set; }
        public decimal EstimatedDurationHours { get; set; }

        // Navigation properties
        public Location DepartureLocation { get; set; }
        public Location DestinationLocation { get; set; }
        public ICollection<Schedule> Schedules { get; set; }
    }
}