// TMS.Infrastructure/Services/SeatLockService.cs
using TMS.Application.Interfaces;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    /// <summary>
    /// Thread-safe, in-memory implementation of ISeatLockService.
    /// Registered as Singleton so the lock dictionary is shared across all Hub connections.
    /// </summary>
    public class SeatLockService : ISeatLockService
    {
        // Key: (tripId, seatNumber)  →  SeatLock
        private readonly Dictionary<(int, string), SeatLock> _locks = new();
        private readonly object _sync = new();

        public bool TryLockSeat(int tripId, string seatNumber, string connectionId)
        {
            lock (_sync)
            {
                var key = (tripId, seatNumber);

                if (_locks.TryGetValue(key, out var existing))
                {
                    // Allow the same connection to re-lock (idempotent)
                    if (existing.ConnectionId == connectionId)
                    {
                        existing.LockedAt = DateTime.UtcNow; // refresh
                        return true;
                    }

                    // Expired lock from someone else → take it over
                    if (existing.IsExpired)
                    {
                        _locks[key] = new SeatLock
                        {
                            TripId = tripId,
                            SeatNumber = seatNumber,
                            ConnectionId = connectionId,
                            LockedAt = DateTime.UtcNow
                        };
                        return true;
                    }

                    // Active lock held by a different connection
                    return false;
                }

                // No existing lock → acquire
                _locks[key] = new SeatLock
                {
                    TripId = tripId,
                    SeatNumber = seatNumber,
                    ConnectionId = connectionId,
                    LockedAt = DateTime.UtcNow
                };
                return true;
            }
        }

        public void ReleaseSeat(int tripId, string seatNumber, string connectionId)
        {
            lock (_sync)
            {
                var key = (tripId, seatNumber);
                if (_locks.TryGetValue(key, out var existing) &&
                    existing.ConnectionId == connectionId)
                {
                    _locks.Remove(key);
                }
            }
        }

        public IEnumerable<(int tripId, string seatNumber)> ReleaseAllByConnection(string connectionId)
        {
            lock (_sync)
            {
                var toRemove = _locks
                    .Where(kv => kv.Value.ConnectionId == connectionId)
                    .Select(kv => kv.Key)
                    .ToList();

                foreach (var key in toRemove)
                    _locks.Remove(key);

                return toRemove.Select(k => (k.Item1, k.Item2)).ToList();
            }
        }

        public IReadOnlyList<(string seatNumber, string connectionId, bool isExpired)> GetLockedSeats(int tripId)
        {
            lock (_sync)
            {
                return _locks
                    .Where(kv => kv.Key.Item1 == tripId)
                    .Select(kv => (kv.Value.SeatNumber, kv.Value.ConnectionId, kv.Value.IsExpired))
                    .ToList();
            }
        }

        public IEnumerable<(int tripId, string seatNumber)> PurgeExpiredLocks()
        {
            lock (_sync)
            {
                var expired = _locks
                    .Where(kv => kv.Value.IsExpired)
                    .Select(kv => kv.Key)
                    .ToList();

                foreach (var key in expired)
                    _locks.Remove(key);

                return expired.Select(k => (k.Item1, k.Item2)).ToList();
            }
        }

        public bool IsLockedByOther(int tripId, string seatNumber, string connectionId)
        {
            lock (_sync)
            {
                var key = (tripId, seatNumber);
                return _locks.TryGetValue(key, out var existing) &&
                       existing.ConnectionId != connectionId &&
                       !existing.IsExpired;
            }
        }

        /// <summary>
        /// Remove the temporary locks for seats that were just permanently booked.
        /// Removes regardless of who technically holds the lock — by the time save()
        /// succeeds client-side, the booking agent's own connection should hold
        /// these locks anyway (they were set via LockSeat while selecting seats).
        /// </summary>
        public void ConfirmBooked(int tripId, IEnumerable<string> seatNumbers, string connectionId)
        {
            lock (_sync)
            {
                foreach (var seatNumber in seatNumbers)
                {
                    _locks.Remove((tripId, seatNumber));
                }
            }
        }
    }
}