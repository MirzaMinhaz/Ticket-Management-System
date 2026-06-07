// TMS.Domain/Entities/SeatLock.cs
namespace TMS.Domain.Entities
{
    /// <summary>
    /// Represents a temporary in-memory seat lock held by a connected user.
    /// This is NOT persisted to the database — it lives only in memory via SeatLockService.
    /// A lock expires automatically after LockDurationSeconds.
    /// </summary>
    public class SeatLock
    {
        public int TripId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;

        /// <summary>SignalR connection ID of the user who holds the lock.</summary>
        public string ConnectionId { get; set; } = string.Empty;

        /// <summary>UTC time when this lock was acquired.</summary>
        public DateTime LockedAt { get; set; } = DateTime.UtcNow;

        /// <summary>How long (seconds) a lock is valid before it auto-expires.</summary>
        public static readonly int LockDurationSeconds = 300; // 5 minutes

        public bool IsExpired =>
            (DateTime.UtcNow - LockedAt).TotalSeconds > LockDurationSeconds;
    }
}