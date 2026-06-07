// TMS.Domain/Entities/Ticket.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Ticket : BaseEntity<int>
    {
        public int UserId { get; set; }

        // CRITICAL CHANGE: ScheduleId সরিয়ে TripId যোগ করা হয়েছে
        public int TripId { get; set; }
        public Trip Trip { get; set; }

        public string? SeatCode { get; set; }
        public string SeatNumber { get; set; }

        public int BookingCounterId { get; set; }
        public int DepartureCounterId { get; set; }
        public int ArrivalCounterId { get; set; }

        public string TicketCode { get; set; }
        public string PassengerName { get; set; }
        public string PassengerContact { get; set; }
        public decimal FarePaid { get; set; }
        public DateTime BookingDateTime { get; set; }
        public string Status { get; set; }

        // Navigation properties
        public User User { get; set; }
        public TicketCounter BookingCounter { get; set; }
        public TicketCounter DepartureCounter { get; set; }
        public TicketCounter ArrivalCounter { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}