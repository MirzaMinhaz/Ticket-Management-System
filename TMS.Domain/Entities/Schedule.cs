// TMS.Domain/Entities/Schedule.cs
using System;
using System.Collections.Generic; // For ICollection

namespace TMS.Domain.Entities
{
    public class Schedule : BaseEntity
    {
        public Guid RouteId { get; set; } // FK to Route
        public Guid VehicleId { get; set; } // FK to Vehicle
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public decimal BaseFare { get; set; }
        public string Status { get; set; } // 'Scheduled', 'Departed', 'Cancelled'

        // Navigation properties
        public Route Route { get; set; }
        public Vehicle Vehicle { get; set; }
        public ICollection<Seat> Seats { get; set; } // A schedule has many seats
        public ICollection<Ticket> Tickets { get; set; } // A schedule can have many tickets
    }
}