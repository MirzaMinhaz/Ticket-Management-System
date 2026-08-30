using System;

namespace TMS.Application.Exceptions
{
    public class AccountLockedException : ApplicationException
    {
        public TimeSpan RemainingTime { get; }

        public AccountLockedException(string message, TimeSpan remainingTime) : base(message)
        {
            RemainingTime = remainingTime;
        }
    }
}