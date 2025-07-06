// TMS.Domain/Entities/Comment.cs
using System;

namespace TMS.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public Guid TicketId { get; set; } // FK to Ticket
        public Guid UserId { get; set; } // FK to User
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Ticket Ticket { get; set; }
        public User User { get; set; }
    }
}