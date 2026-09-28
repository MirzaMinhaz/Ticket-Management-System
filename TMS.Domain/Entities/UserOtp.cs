using System;

namespace TMS.Domain.Entities
{
    public class UserOtp : BaseEntity<int>
    {
        public int UserId { get; set; }

        public string OtpHash { get; set; }

        public string OtpType { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; }

        public int AttemptCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? LastModifiedAt { get; set; }

        public string LastModifiedBy { get; set; }

        // Navigation Property
        public User User { get; set; }
    }
}