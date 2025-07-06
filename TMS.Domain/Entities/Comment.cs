// TMS.Domain/Entities/Comment.cs
using TMS.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty; // <-- Initialize here
    // OR make it nullable: public string? Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ticket Ticket { get; set; }
    public User User { get; set; }
}