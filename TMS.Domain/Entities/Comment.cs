// TMS.Domain/Entities/Comment.cs
using System;

namespace TMS.Domain.Entities
{
    public class Comment : BaseEntity<int> // Inherit from BaseEntity<int>
    {
        // Primary key is 'Id' from BaseEntity<int>

        public int TicketId { get; set; } // Change from Guid to int
        public int UserId { get; set; }   // Change from Guid to int
        public string Content { get; set; } = string.Empty;
        // public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Already in BaseEntity

        public Ticket Ticket { get; set; }
        public User User { get; set; }
    }
}