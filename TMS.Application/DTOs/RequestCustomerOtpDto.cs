using System.ComponentModel.DataAnnotations;

namespace TMS.Application.DTOs
{
    public class RequestCustomerOtpDto
    {
        [Required]
        public string Username { get; set; }
    }
}