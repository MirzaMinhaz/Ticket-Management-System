// TMS.Domain/Entities/User.cs
using System;
using System.Collections.Generic; // For ICollection

namespace TMS.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Simple role for now
        // ... other user-specific properties

        // Navigation property: A user can have many tickets
        public ICollection<Ticket> Tickets { get; set; }
        // Navigation property: A user can make many comments
        public ICollection<Comment> Comments { get; set; }
    }
}