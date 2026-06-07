// TMS.API/BackgroundServices/SeatLockPurgeService.cs
using Microsoft.AspNetCore.SignalR;
using TMS.API.Hubs;
using TMS.Application.Interfaces;

namespace TMS.WebAPI.BackgroundServices
{
    /// <summary>
    /// Runs every 60 seconds and purges seat locks that have exceeded
    /// SeatLock.LockDurationSeconds (5 minutes). Broadcasts SeatReleased
    /// to the appropriate trip groups so clients update in real time.
    /// </summary>
    public class SeatLockPurgeService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SeatLockPurgeService> _logger;
        private static readonly TimeSpan _interval = TimeSpan.FromSeconds(60);

        public SeatLockPurgeService(IServiceProvider serviceProvider, ILogger<SeatLockPurgeService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SeatLockPurgeService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_interval, stoppingToken);

                try
                {
                    // Both ISeatLockService (Singleton) and IHubContext can be resolved directly
                    using var scope = _serviceProvider.CreateScope();
                    var lockService = scope.ServiceProvider.GetRequiredService<ISeatLockService>();
                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<SeatHub>>();

                    var released = lockService.PurgeExpiredLocks().ToList();

                    foreach (var (tripId, seatNumber) in released)
                    {
                        await hubContext.Clients.Group($"trip-{tripId}")
                            .SendAsync("SeatReleased", tripId, seatNumber, stoppingToken);

                        _logger.LogInformation(
                            "Expired lock purged: seat {Seat} on trip {Trip}.", seatNumber, tripId);
                    }
                }
                catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError(ex, "Error in SeatLockPurgeService.");
                }
            }
        }
    }
}