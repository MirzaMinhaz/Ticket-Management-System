using System;
using System.ComponentModel.DataAnnotations;

namespace TMS.Application.DTOs
{
    // DTO for retrieving a Vehicle
    public class VehicleDto
    {
        public int Id { get; set; }
        public string OperatorCode { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
        public int Capacity { get; set; }
        public string VehicleCode { get; set; }

        // --- Added New Columns ---
        public string ACType { get; set; }      // AC / Non-AC
        public string BusCategory { get; set; } // Sleeper / Seater
        public string DeckLevel { get; set; }   // Single / Double

        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }

    // DTO for creating a new Vehicle
    // DTO for creating a new Vehicle
    public class CreateVehicleDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Type { get; set; }

        [Required]
        public string OperatorCode { get; set; }

        [Required]
        [StringLength(100)]
        public string Model { get; set; }

        [Required]
        [StringLength(20)]
        public string LicensePlate { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Capacity { get; set; }

        // Bus-only — null for Train
        public string? ACType { get; set; }      // ← removed [Required], made nullable
        public string? BusCategory { get; set; } // ← same
        public string? DeckLevel { get; set; }   // ← same
    }

    // DTO for updating an existing Vehicle
    public class UpdateVehicleDto
    {
        public string OperatorCode { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Type { get; set; }

        [Required]
        [StringLength(100)]
        public string Model { get; set; }

        [Required]
        [StringLength(20)]
        public string LicensePlate { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Capacity { get; set; }

        // --- Added New Fields for Update ---
        public string? ACType { get; set; }
        public string? BusCategory { get; set; }
        public string? DeckLevel { get; set; }

        public bool IsActive { get; set; }
    }
}