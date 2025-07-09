// TMS.Domain/Entities/User.cs
using System;
using System.Collections.Generic;

namespace TMS.Domain.Entities
{
    public class User : BaseEntity<int> // Inherit from BaseEntity<int>
    {
        // Primary key is 'Id' from BaseEntity<int>

        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Simple string role. Consider a UserId to RoleId FK if you have a UserRole entity
        public string UserCode { get; set; } // e.g., USR-0001, USR-0010

        // Navigation property: A user can have many tickets
        public ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
        // Navigation property: A user can make many comments
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        public User()
        {
            Tickets = new HashSet<Ticket>();
            Comments = new HashSet<Comment>();
        }
    }
}