// TMS.Domain/Entities/Schedule.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Schedule : BaseEntity<int> // Inherit from BaseEntity<int>
    {
        // Primary key is 'Id' from BaseEntity<int>

        public int RouteId { get; set; } // FK to Route (int)
        public int VehicleId { get; set; } // FK to Vehicle (int)
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public decimal BaseFare { get; set; }
        public string Status { get; set; } // 'Scheduled', 'Departed', 'Cancelled'
        public string ScheduleCode { get; set; } // e.g., SCD-0001, SCD-0010

        // Navigation properties
        public Route Route { get; set; }
        public Vehicle Vehicle { get; set; }
        public ICollection<Seat> Seats { get; set; } = new HashSet<Seat>(); // A schedule has many seats (per schedule, not vehicle)
        public ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>(); // A schedule can have many tickets

        public Schedule()
        {
            Seats = new HashSet<Seat>();
            Tickets = new HashSet<Ticket>();
        }
    }
}