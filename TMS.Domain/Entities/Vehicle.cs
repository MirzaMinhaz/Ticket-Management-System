// TMS.Domain/Entities/Vehicle.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Vehicle : BaseEntity<int> // Inherit from BaseEntity<int>
    {
        // Primary key is 'Id' from BaseEntity<int>

        public int OperatorId { get; set; } // FK to Operator (int)
        public string Type { get; set; } // 'Bus', 'Train', 'Plane'
        public string Model { get; set; }
        public string LicensePlate { get; set; }
        public int Capacity { get; set; }
        public string VehicleCode { get; set; } // e.g., VHC-0001, VHC-0010

        // Navigation properties
        public Operator Operator { get; set; }
        public ICollection<Schedule> Schedules { get; set; } = new HashSet<Schedule>();

        public Vehicle()
        {
            Schedules = new HashSet<Schedule>();
        }
    }
}