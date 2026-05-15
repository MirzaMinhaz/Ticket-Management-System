using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class Vehicle : BaseEntity<int>
    {
        public string? OperatorCode { get; set; } // ✅ Rename from OperatorId to OperatorCode

        public string Type { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
        public int Capacity { get; set; }
        public string? VehicleCode { get; set; }
        public string? DeckLevel { get; set; }   // e.g., Single Deck, Double Deck
        public string? BusCategory { get; set; } // e.g., Sleeper, Seater
        public string? ACType { get; set; }      // e.g., AC, Non-AC

        public Operator Operator { get; set; }
        public ICollection<Schedule> Schedules { get; set; } = new HashSet<Schedule>();
    }
}
