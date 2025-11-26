// TMS.Domain/Entities/Seat.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Seat : BaseEntity<int> 
    {

        public int ScheduleId { get; set; }
        public string SeatNumber { get; set; } 
        public bool IsBooked { get; set; } = false; 
        public string SeatCode { get; set; } 
        public Schedule Schedule { get; set; }

    }
}