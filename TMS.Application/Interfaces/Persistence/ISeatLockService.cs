// TMS.Application/Interfaces/ISeatLockService.cs
namespace TMS.Application.Interfaces
{
    /// <summary>
    /// Manages temporary seat locks so that multiple concurrent users cannot
    /// select the same seat simultaneously.
    ///
    /// Flow:
    ///   1. User opens seat panel → calls GetLockedSeats to see what's already locked.
    ///   2. User clicks a seat    → calls TryLockSeat.
    ///                              Returns true  → lock acquired, broadcast to others.
    ///                              Returns false → seat already locked/booked.
    ///   3. User deselects seat   → calls ReleaseSeat.
    ///   4. User disconnects      → ReleaseAllByConnection cleans up their locks.
    ///   5. Background timer      → PurgeExpiredLocks removes stale locks.
    /// </summary>
    public interface ISeatLockService
    {
        /// <summary>
        /// Try to lock a seat for the given connection.
        /// Returns true if the lock was successfully acquired.
        /// Returns false if another connection already holds it (or it is already booked).
        /// </summary>
        bool TryLockSeat(int tripId, string seatNumber, string connectionId);

        /// <summary>
        /// Release a specific seat lock held by a connection.
        /// No-op if the connection does not own the lock.
        /// </summary>
        void ReleaseSeat(int tripId, string seatNumber, string connectionId);

        /// <summary>
        /// Release ALL locks held by a connection (called on disconnect).
        /// Returns the list of (tripId, seatNumber) pairs that were released
        /// so the Hub can broadcast the release to other clients.
        /// </summary>
        IEnumerable<(int tripId, string seatNumber)> ReleaseAllByConnection(string connectionId);

        /// <summary>
        /// Returns all currently locked seats for a trip (including expired ones
        /// that have not yet been purged — callers should call PurgeExpiredLocks first).
        /// </summary>
        IReadOnlyList<(string seatNumber, string connectionId, bool isExpired)> GetLockedSeats(int tripId);

        /// <summary>
        /// Remove all expired locks globally and return the released (tripId, seatNumber) pairs
        /// so the Hub can broadcast them.
        /// </summary>
        IEnumerable<(int tripId, string seatNumber)> PurgeExpiredLocks();

        /// <summary>
        /// Returns true if the given seat is currently locked by ANY connection other than
        /// the one provided (so a user can re-lock their own seat after reconnect).
        /// </summary>
        bool IsLockedByOther(int tripId, string seatNumber, string connectionId);

        void ConfirmBooked(int tripId, IEnumerable<string> seatNumbers, string connectionId);
    }
}