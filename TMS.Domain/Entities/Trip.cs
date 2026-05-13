// TMS.Domain/Entities/Trip.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Trip : BaseEntity<int>
    {
        public int ScheduleId { get; set; }
        public DateTime TripDate { get; set; } 
        public string Status { get; set; }

        // Navigation Properties
        public Schedule Schedule { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

        public Trip()
        {
            Tickets = new HashSet<Ticket>();
        }
    }
}