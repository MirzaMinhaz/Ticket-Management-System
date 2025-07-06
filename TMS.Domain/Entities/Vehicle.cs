// TMS.Domain/Entities/Vehicle.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        public Guid OperatorId { get; set; } // FK to Operator
        public string Type { get; set; } // 'Bus'
        public string Model { get; set; }
        public string LicensePlate { get; set; }
        public int Capacity { get; set; }

        // Navigation properties
        public Operator Operator { get; set; }
        public ICollection<Schedule> Schedules { get; set; }
    }
}