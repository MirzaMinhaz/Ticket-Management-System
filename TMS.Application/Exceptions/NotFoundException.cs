// TMS.Application/Exceptions/NotFoundException.cs
using System;

namespace TMS.Application.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string message) : base(message) { }

        // You can add constructors for specific scenarios if needed
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.") { }
    }
}