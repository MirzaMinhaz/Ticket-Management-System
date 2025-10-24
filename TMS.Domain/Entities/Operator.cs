using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Operator : BaseEntity<int> // Primary key is 'Id'
    {
        public string Name { get; set; }
        public string Type { get; set; } // e.g., 'Bus Company', 'Airline'

        public string OperatorCode { get; set; } // e.g., 'OPR-0001', must be unique

        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; } = new HashSet<Vehicle>();
    }
}
