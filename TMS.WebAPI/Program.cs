// TMS.WebAPI/Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TMS.Application;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Application.Services;
using TMS.Infrastructure;
using TMS.Infrastructure.Persistence;
using TMS.Infrastructure.Persistence.Repositories;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TMS.API.Hubs;
using TMS.WebAPI.BackgroundServices;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog Logger Setup ──────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

// Attach Serilog as the Host logging provider
builder.Host.UseSerilog();

try
{
    Log.Information("Starting Ticket Management System (TMS) Web API...");

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpContextAccessor();

    // ── Application & Infrastructure ─────────────────────────────────────────────
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    builder.Services.AddDbContext<TicketManagementDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // ── SignalR ───────────────────────────────────────────────────────────────────
    // CRITICAL: Must be added before Build()
    builder.Services.AddSignalR();

    // ── Seat lock background purge ────────────────────────────────────────────────
    builder.Services.AddHostedService<SeatLockPurgeService>();

    // ── DB warm-up (mitigates cold-start delay on first request after restart) ────
    builder.Services.AddHostedService<DatabaseWarmupService>();

    // ── JWT Auth ──────────────────────────────────────────────────────────────────
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                                                   Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };

            // CRITICAL for SignalR: allow token from query string
            // SignalR sends the JWT as ?access_token=... on the WebSocket upgrade request
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization();

    // ── CORS ──────────────────────────────────────────────────────────────────────
    // CRITICAL: SignalR requires AllowCredentials() + specific origin (not wildcard)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular", policy =>
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials());   // ← REQUIRED for SignalR WebSocket handshake
    });

    // ── Rate Limiting (brute-force login protection) ───────────────────────────────
    builder.Services.AddRateLimiter(options =>
    {
        // Requests that get rejected return 429 Too Many Requests
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Policy: per IP address, max 5 login attempts per 5-minute window.
        // Fixed window = simple, predictable — resets fully every 5 minutes.
        options.AddPolicy("LoginPolicy", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(5),
                    QueueLimit = 0 // don't queue extra requests — reject immediately
                }));

        options.OnRejected = (context, cancellationToken) =>
        {
            var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            Log.Warning("Rate limit exceeded for IP: {IpAddress} on path: {Path}", ip, context.HttpContext.Request.Path);
            return ValueTask.CompletedTask;
        };

    });

    // ─────────────────────────────────────────────────────────────────────────────
    var app = builder.Build();

    // ── Serilog Request Logging ──────────────────────────────────────────────────
    // Intercepts and logs all incoming HTTP requests automatically
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // CRITICAL ORDER: CORS → Auth → Endpoints
    app.UseCors("AllowAngular");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRateLimiter();

    app.MapControllers();

    // ── SignalR Hub endpoint ──────────────────────────────────────────────────────
    app.MapHub<SeatHub>("/hubs/seats");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "TMS Web API Host terminated unexpectedly!");
}
finally
{
    // Flushes all pending log events to sinks before application shutdown
    Log.CloseAndFlush();
}