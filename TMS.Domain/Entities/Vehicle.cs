using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Vehicle : BaseEntity<int>
    {
        public int? OperatorId { get; set; } // ✅ Nullable

        public string Type { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
        public int Capacity { get; set; }
        public string VehicleCode { get; set; }

        public Operator Operator { get; set; }
        public ICollection<Schedule> Schedules { get; set; } = new HashSet<Schedule>();

        public Vehicle()
        {
            Schedules = new HashSet<Schedule>();
        }
    }
}
