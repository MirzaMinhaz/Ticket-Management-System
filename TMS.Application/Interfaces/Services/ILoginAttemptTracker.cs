using System;

namespace TMS.Application.Interfaces.Services
{
    public interface ILoginAttemptTracker
    {
        bool IsLocked(string username);
        TimeSpan? GetRemainingLockTime(string username);
        void RegisterFailedAttempt(string username);
        void ResetAttempts(string username);
    }
}