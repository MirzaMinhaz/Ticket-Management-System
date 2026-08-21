using System;

namespace TMS.Application.Exceptions
{
    // Thrown when a request is authenticated (valid credentials) but not
    // authorized for the specific action/portal being accessed.
    public class ForbiddenException : ApplicationException
    {
        public ForbiddenException(string message) : base(message) { }
    }
}