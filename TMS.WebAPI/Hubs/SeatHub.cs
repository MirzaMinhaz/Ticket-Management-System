// TMS.API/Hubs/SeatHub.cs
using Microsoft.AspNetCore.SignalR;
using TMS.Application.Interfaces;

namespace TMS.API.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time seat selection locking.
    ///
    /// Client → Server methods (called via HubConnection.InvokeAsync):
    ///   JoinTrip(tripId)                   – subscribe to a trip's seat events
    ///   LeaveTrip(tripId)                  – unsubscribe
    ///   LockSeat(tripId, seatNumber)        – try to lock a seat
    ///   ReleaseSeat(tripId, seatNumber)     – release a seat lock
    ///   GetLockedSeats(tripId)              – get current locked seats snapshot
    ///   ConfirmBooking(tripId, seatNumbers) – a ticket was just saved for these seats
    ///
    /// Server → Client events (listened via HubConnection.On):
    ///   SeatLocked(tripId, seatNumber, connectionId)     – a seat was just locked
    ///   SeatReleased(tripId, seatNumber)                 – a seat was just released
    ///   LockedSeatsSnapshot(tripId, lockedSeats[])       – full snapshot on join
    ///   LockFailed(tripId, seatNumber, reason)           – lock attempt failed
    ///   SeatsBooked(tripId, seatNumbers[])                – seats are now permanently booked
    /// </summary>
    public class SeatHub : Hub
    {
        private readonly ISeatLockService _lockService;
        private readonly ILogger<SeatHub> _logger;

        public SeatHub(ISeatLockService lockService, ILogger<SeatHub> logger)
        {
            _lockService = lockService;
            _logger = logger;
        }

        // ── Connection lifecycle ────────────────────────────────────────────

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var released = _lockService.ReleaseAllByConnection(Context.ConnectionId);

            foreach (var (tripId, seatNumber) in released)
            {
                await Clients.Group($"trip-{tripId}")
                    .SendAsync("SeatReleased", tripId, seatNumber);

                _logger.LogInformation(
                    "Seat {Seat} on trip {Trip} released — connection {Conn} disconnected.",
                    seatNumber, tripId, Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // ── Client → Server ────────────────────────────────────────────────

        /// <summary>Join the SignalR group for a trip to receive seat events.</summary>
        public async Task JoinTrip(int tripId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"trip-{tripId}");
            _logger.LogDebug("Connection {Conn} joined trip {Trip}.", Context.ConnectionId, tripId);

            // Immediately send a snapshot of all currently locked seats
            var locked = _lockService.GetLockedSeats(tripId)
                .Where(s => !s.isExpired)
                .Select(s => new { seatNumber = s.seatNumber, connectionId = s.connectionId })
                .ToList();

            await Clients.Caller.SendAsync("LockedSeatsSnapshot", tripId, locked);
        }

        /// <summary>Leave the SignalR group for a trip.</summary>
        public async Task LeaveTrip(int tripId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"trip-{tripId}");
        }

        /// <summary>
        /// Try to lock a seat on behalf of the calling connection.
        /// Broadcasts SeatLocked to the trip group on success,
        /// or sends LockFailed back to the caller on failure.
        /// </summary>
        public async Task LockSeat(int tripId, string seatNumber)
        {
            var success = _lockService.TryLockSeat(tripId, seatNumber, Context.ConnectionId);

            if (success)
            {
                _logger.LogInformation(
                    "Seat {Seat} on trip {Trip} locked by {Conn}.",
                    seatNumber, tripId, Context.ConnectionId);

                // Broadcast to everyone in the trip group (including caller)
                await Clients.Group($"trip-{tripId}")
                    .SendAsync("SeatLocked", tripId, seatNumber, Context.ConnectionId);
            }
            else
            {
                await Clients.Caller
                    .SendAsync("LockFailed", tripId, seatNumber, "Seat is already selected by another user.");
            }
        }

        /// <summary>Release a seat lock held by the calling connection.</summary>
        public async Task ReleaseSeat(int tripId, string seatNumber)
        {
            _lockService.ReleaseSeat(tripId, seatNumber, Context.ConnectionId);

            await Clients.Group($"trip-{tripId}")
                .SendAsync("SeatReleased", tripId, seatNumber);

            _logger.LogDebug(
                "Seat {Seat} on trip {Trip} released by {Conn}.",
                seatNumber, tripId, Context.ConnectionId);
        }

        /// <summary>Return a fresh snapshot of locked seats for a trip.</summary>
        public async Task GetLockedSeats(int tripId)
        {
            var locked = _lockService.GetLockedSeats(tripId)
                .Where(s => !s.isExpired)
                .Select(s => new { seatNumber = s.seatNumber, connectionId = s.connectionId })
                .ToList();

            await Clients.Caller.SendAsync("LockedSeatsSnapshot", tripId, locked);
        }

        /// <summary>
        /// Called by a client the instant a ticket save/update succeeds for the
        /// given seats. This is the critical piece that closes the double-booking
        /// window: it removes the temporary locks (the seats no longer need one —
        /// they're now permanently reserved in the ticket/booking table) and
        /// broadcasts to every client on this trip that the seats are booked, so
        /// their UI flips to "Taken" immediately — no page refresh required.
        ///
        /// Without this, a save only ever triggers ReleaseSeat on cleanup, which
        /// broadcasts SeatReleased and makes the seat look free to everyone else
        /// until they happen to refetch booked seats from the database.
        /// </summary>
        public async Task ConfirmBooking(int tripId, List<string> seatNumbers)
        {
            if (seatNumbers == null || seatNumbers.Count == 0) return;

            _lockService.ConfirmBooked(tripId, seatNumbers, Context.ConnectionId);

            await Clients.Group($"trip-{tripId}")
                .SendAsync("SeatsBooked", tripId, seatNumbers);

            _logger.LogInformation(
                "Seats {Seats} on trip {Trip} confirmed booked by {Conn}.",
                string.Join(",", seatNumbers), tripId, Context.ConnectionId);
        }
    }
}