// TMS.Domain/Entities/Schedule.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Schedule : BaseEntity<int>
    {
        public int RouteId { get; set; }
        public int VehicleId { get; set; }
        public DateTime DepartureDateTime { get; set; } // এটি হবে মাস্টার টাইম
        public DateTime ArrivalDateTime { get; set; }
        public decimal BaseFare { get; set; }
        public string Status { get; set; }
        public string ScheduleCode { get; set; }

        // Navigation properties
        public Route Route { get; set; }
        public Vehicle Vehicle { get; set; }

        // এখন এটি সরাসরি টিকেটের বদলে ট্রিপের সাথে যুক্ত
        public ICollection<Trip> Trips { get; set; } = new HashSet<Trip>();
        public ICollection<Seat> Seats { get; set; } = new HashSet<Seat>();

        public Schedule()
        {
            Trips = new HashSet<Trip>();
            Seats = new HashSet<Seat>();
        }
    }
}