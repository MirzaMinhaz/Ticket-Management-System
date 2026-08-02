using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TMS.Application.DTOs
{
    public class RegisterRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Used only by the [Authorize(Roles = "Admin")]-protected staff registration
    // endpoint. Role is required here and validated server-side against
    // Roles.AssignableStaffRoles — never trust a role string from an
    // unauthenticated/self-serve caller.
    public class RegisterStaffRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class LoginRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
    }
}