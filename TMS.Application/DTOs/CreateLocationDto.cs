using System.ComponentModel.DataAnnotations; // ADDED/CONFIRMED

namespace TMS.Application.DTOs
{
    public class CreateLocationDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Location name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [StringLength(50, ErrorMessage = "Location type cannot exceed 50 characters.")]
        public string Type { get; set; }

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string Address { get; set; }
    }
}