using System;
using System.ComponentModel.DataAnnotations;

namespace TMS.Application.DTOs
{
    // DTO for retrieving a Vehicle
    public class VehicleDto
    {
        public int Id { get; set; }

        public string OperatorCode { get; set; } // ✅ Changed from OperatorId

        public string Type { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
        public int Capacity { get; set; }
        public string VehicleCode { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }


    // DTO for creating a new Vehicle
    public class CreateVehicleDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Type { get; set; }

        [Required] // ✅ Ensures operator is selected
        public string OperatorCode { get; set; } // ✅ Added to link vehicle to operator

        [Required]
        [StringLength(100)]
        public string Model { get; set; }

        [Required]
        [StringLength(20)]
        public string LicensePlate { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Capacity { get; set; }
    }


    // DTO for updating an existing Vehicle
    public class UpdateVehicleDto
    {
        public string OperatorCode { get; set; } // Optional

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

        public bool IsActive { get; set; }
    }

}
