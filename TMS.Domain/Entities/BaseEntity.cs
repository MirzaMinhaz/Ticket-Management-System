// TMS.Domain/Entities/BaseEntity.cs
using System;

namespace TMS.Domain.Entities
{
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; set; } // The primary key (will be int for most entities)

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }
    }
}