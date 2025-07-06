// TMS.Domain/Entities/BaseEntity.cs
using System; // Make sure System is imported for Guid and DateTime

namespace TMS.Domain.Entities
{
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; } // Nullable
        public string CreatedBy { get; set; } // Nullable
        public string LastModifiedBy { get; set; } // Nullable
    }

    // A non-generic BaseEntity for common use with Guid IDs
    public abstract class BaseEntity : BaseEntity<Guid>
    {
        // No additional properties needed here, just inherits BaseEntity<Guid>
    }
}