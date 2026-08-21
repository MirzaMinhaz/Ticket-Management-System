using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TMS.Infrastructure.Persistence;

namespace TMS.WebAPI.BackgroundServices
{
    // Runs once at application startup, in the background, to pay the
    // "first request is slow" cost (JIT compilation, EF Core model build,
    // DB connection pool warm-up) before any real user hits the API —
    // instead of the first real login absorbing that 2-7 second delay.
    public class DatabaseWarmupService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DatabaseWarmupService> _logger;

        // Hosted services are registered as singletons, but DbContext is
        // scoped — so we resolve it through a scope factory rather than
        // injecting DbContext directly.
        public DatabaseWarmupService(IServiceScopeFactory scopeFactory, ILogger<DatabaseWarmupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Fire-and-forget: don't block app startup/Run() waiting on this.
            _ = WarmUpAsync(cancellationToken);
            return Task.CompletedTask;
        }

        private async Task WarmUpAsync(CancellationToken cancellationToken)
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<TicketManagementDbContext>();

                // A cheap, real query against the exact table/path the login
                // flow uses (Users, no tracking) — this forces the DB
                // connection to open, the connection pool to establish, and
                // EF Core to build/cache the model for this entity, all
                // before a real user's first login request pays that cost.
                await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == -1, cancellationToken);

                stopwatch.Stop();
                _logger.LogInformation(
                    "Database warm-up completed in {ElapsedMs} ms.",
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                // Warm-up failing should never crash the app or block startup —
                // worst case, the first real request just pays the cold-start
                // cost as before. Log it so it's visible, not silent.
                _logger.LogWarning(ex, "Database warm-up failed (non-fatal) — first real request may be slower than usual.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}