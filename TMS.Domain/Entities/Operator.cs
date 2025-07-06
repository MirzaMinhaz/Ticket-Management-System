// TMS.Domain/Entities/Operator.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Operator : BaseEntity
    {
        public string Name { get; set; }
        public string Type { get; set; } // e.g., 'Bus Company'
        // ... other operator details

        public ICollection<Vehicle> Vehicles { get; set; }
    }
}