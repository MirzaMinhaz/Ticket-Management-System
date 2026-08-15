// TMS.Application/Exceptions/SeatConflictException.cs
namespace TMS.Application.Exceptions
{
    /// <summary>
    /// Thrown when a ticket save request would double-book one or more seats
    /// that another request already claimed for the same trip — surfaced to
    /// the client as HTTP 409 Conflict, distinct from validation errors (400)
    /// and generic failures (500), so the frontend can react specifically
    /// (refresh the seat map instead of just showing a generic error).
    /// </summary>
    public class SeatConflictException : Exception
    {
        public IReadOnlyList<string> ConflictingSeats { get; }

        public SeatConflictException(IEnumerable<string> conflictingSeats)
            : base($"Seat(s) already booked: {string.Join(", ", conflictingSeats)}")
        {
            ConflictingSeats = conflictingSeats.ToList();
        }
    }
}