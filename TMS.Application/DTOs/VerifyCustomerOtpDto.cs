using System.ComponentModel.DataAnnotations;

namespace TMS.Application.DTOs
{
    public class VerifyCustomerOtpDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Otp { get; set; }
    }
}