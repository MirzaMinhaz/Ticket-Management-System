// TMS.Application/DTOs/LocationDto.cs
using System; // Required for DateTime
using System.ComponentModel.DataAnnotations; // <<<--- THIS IS ESSENTIAL FOR [Required] and [StringLength]

namespace TMS.Application.DTOs
{
    public class LocationDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Location code is required.")]
        [StringLength(10, ErrorMessage = "Location code cannot exceed 10 characters.")]
        public string LocationCode { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Location name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [StringLength(50, ErrorMessage = "Location type cannot exceed 50 characters.")]
        public string Type { get; set; }

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
    }
}