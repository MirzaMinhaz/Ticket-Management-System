// TMS.Domain/Entities/BaseEntity.cs
using System;

namespace TMS.Domain.Entities
{
    // Make BaseEntity generic to allow different ID types
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; set; } // The primary key property

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
    }
}