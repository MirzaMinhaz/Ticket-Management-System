using System;
using System.ComponentModel.DataAnnotations;

namespace TMS.Application.DTOs
{
    public class TicketCounterDto
    {
        public int Id { get; set; } // Change from Guid to int (this is the actual DB ID)
        public string LocationCode { get; set; } // Change from Guid to int
        public string CounterName { get; set; }
        public string CounterCode { get; set; } // The formatted string ID like TCO-001
        public string AddressDetails { get; set; }
        public string ContactNumber { get; set; }
        public string? OperatingHours { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        // Optional: Include Location Name for display purposes
        public string LocationName { get; set; }
    }

    public class CreateTicketCounterDto
    {
        [Required]
        public string LocationCode { get; set; } // Change from Guid to int
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string CounterName { get; set; }
        // CounterCode is generated, so it's not in Create DTO
        [StringLength(250)]
        public string AddressDetails { get; set; }
        [StringLength(20)]
        public string ContactNumber { get; set; }
        [StringLength(100)]
        public string? OperatingHours { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateTicketCounterDto
    {
        [Required]
        public string LocationCode { get; set; } // Change from Guid to int
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string CounterName { get; set; }
        // CounterCode is not updated directly via DTO, it's derived
        [StringLength(250)]
        public string AddressDetails { get; set; }
        [StringLength(20)]
        public string ContactNumber { get; set; }
        [StringLength(100)]
        public string? OperatingHours { get; set; }
        public bool IsActive { get; set; }
    }
}