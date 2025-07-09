// TMS.Domain/Entities/Operator.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Operator : BaseEntity<int> // Inherit from BaseEntity<int>
    {
        // Primary key is 'Id' from BaseEntity<int>

        public string Name { get; set; }
        public string Type { get; set; } // e.g., 'Bus Company', 'Airline'
        public string OperatorCode { get; set; } // e.g., OPR-0001, OPR-0010

        // ... other operator details

        public ICollection<Vehicle> Vehicles { get; set; } = new HashSet<Vehicle>();

        public Operator()
        {
            Vehicles = new HashSet<Vehicle>();
        }
    }
}