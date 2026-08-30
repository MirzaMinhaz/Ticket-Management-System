using Microsoft.Extensions.Caching.Memory;
using System;
using TMS.Application.Interfaces.Services;

namespace TMS.Application.Services
{
    // Tracks failed login attempts per username (case-insensitive), in memory.
    // 5 failed attempts within a 5-minute window → username locked for 5 minutes.
    // This is separate from the IP-based rate limiter: IP limiting protects the
    // server from bulk traffic; this protects a specific account from being
    // brute-forced, regardless of how many people share that IP.
    public class LoginAttemptTracker : ILoginAttemptTracker
    {
        private readonly IMemoryCache _cache;

        private const int MaxAttempts = 5;
        private static readonly TimeSpan AttemptWindow = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(5);

        public LoginAttemptTracker(IMemoryCache cache)
        {
            _cache = cache;
        }

        private static string AttemptKey(string username) => $"login_attempts:{username.Trim().ToLowerInvariant()}";
        private static string LockKey(string username) => $"login_locked:{username.Trim().ToLowerInvariant()}";

        public bool IsLocked(string username)
        {
            return _cache.TryGetValue(LockKey(username), out _);
        }

        public TimeSpan? GetRemainingLockTime(string username)
        {
            if (_cache.TryGetValue(LockKey(username), out DateTimeOffset lockedUntil))
            {
                var remaining = lockedUntil - DateTimeOffset.UtcNow;
                return remaining > TimeSpan.Zero ? remaining : null;
            }
            return null;
        }

        public void RegisterFailedAttempt(string username)
        {
            var key = AttemptKey(username);
            var count = _cache.TryGetValue(key, out int existing) ? existing : 0;
            count++;

            if (count >= MaxAttempts)
            {
                var lockedUntil = DateTimeOffset.UtcNow.Add(LockDuration);
                _cache.Set(LockKey(username), lockedUntil, LockDuration);
                _cache.Remove(key); // lock শুরু হলে কাউন্টার রিসেট
            }
            else
            {
                _cache.Set(key, count, AttemptWindow);
            }
        }

        public void ResetAttempts(string username)
        {
            _cache.Remove(AttemptKey(username));
            _cache.Remove(LockKey(username));
        }
    }
}