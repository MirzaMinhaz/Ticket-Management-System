// TMS.Infrastructure/Persistence/TicketManagementDbContext.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using TMS.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Needed for IHttpContextAccessor
using Microsoft.Extensions.Configuration; // Needed for IConfiguration
using System.Linq; // Needed for .Any() in ApplyAuditInformation (if you refactor to it)

namespace TMS.Infrastructure.Persistence
{
    public class TicketManagementDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor; // To get current user
        private readonly IConfiguration _configuration; // To access configuration if needed

        // Constructor updated to accept IHttpContextAccessor and IConfiguration
        public TicketManagementDbContext(
            DbContextOptions<TicketManagementDbContext> options,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        // --- DbSets (ensure all your tables have a corresponding DbSet) ---
        public DbSet<Location> Locations { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TicketCounter> TicketCounters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }

        // --- Override SaveChanges methods to populate BaseEntity properties ---
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ApplyAuditInformation(); // Call the helper method
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplyAuditInformation(); // Call the helper method
            return base.SaveChanges();
        }

        // Helper method to apply audit information to entities
        private void ApplyAuditInformation()
        {
            // In a real application with authentication, you'd get the user from HttpContext.User.Identity.Name
            // For now, return a default string for development.
            string currentUserName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system_user";

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = currentUserName;
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = currentUserName;
                        break;
                    case EntityState.Modified:
                        // Only update LastModifiedAt and LastModifiedBy if the entity was actually modified
                        // This check is optional but can prevent unnecessary updates if only relationships change
                        // if (entry.OriginalValues.Properties.Any(p => entry.Property(p.Name).IsModified))
                        // {
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = currentUserName;
                        // }
                        break;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // --- Custom Convention for Primary Key Column Names ---
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var idProperty = entityType.FindProperty("Id");
                    if (idProperty != null)
                    {
                        idProperty.SetColumnName(entityType.DisplayName() + "Id");
                    }
                }
            }

            // --- Configure Relationships ---

            modelBuilder.Entity<Location>()
                .HasMany(l => l.TicketCounters)
                .WithOne(tc => tc.Location)
                .HasForeignKey(tc => tc.LocationId);

            modelBuilder.Entity<Operator>()
                .HasMany(o => o.Vehicles)
                .WithOne(v => v.Operator)
                .HasForeignKey(v => v.OperatorId);

            modelBuilder.Entity<Route>()
                .HasMany(r => r.Schedules)
                .WithOne(s => s.Route)
                .HasForeignKey(s => s.RouteId);

            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.Schedules)
                .WithOne(s => s.Vehicle)
                .HasForeignKey(s => s.VehicleId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Tickets)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Important for breaking cascade cycles

            modelBuilder.Entity<Schedule>()
                .HasMany(s => s.Tickets)
                .WithOne(t => t.Schedule)
                .HasForeignKey(t => t.ScheduleId);

            modelBuilder.Entity<Schedule>()
                .HasMany(s => s.Seats)
                .WithOne(s => s.Schedule)
                .HasForeignKey(s => s.ScheduleId);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.BookedSeat)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.SeatId)
                .OnDelete(DeleteBehavior.NoAction); // Important for breaking cascade cycles

            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Comments)
                .WithOne(c => c.Ticket)
                .HasForeignKey(c => c.TicketId);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.BookingCounter)
                .WithMany(tc => tc.BookingTickets)
                .HasForeignKey(t => t.BookingCounterId)
                .IsRequired(false);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.DepartureCounter)
                .WithMany(tc => tc.DepartureTickets)
                .HasForeignKey(t => t.DepartureCounterId)
                .IsRequired(false);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.ArrivalCounter)
                .WithMany(tc => tc.ArrivalTickets)
                .HasForeignKey(t => t.ArrivalCounterId)
                .IsRequired(false);

            // --- Cyclic Cascade Path Fixes ---
            modelBuilder.Entity<Route>()
                .HasOne(r => r.DepartureLocation)
                .WithMany()
                .HasForeignKey(r => r.DepartureLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Route>()
                .HasOne(r => r.DestinationLocation)
                .WithMany()
                .HasForeignKey(r => r.DestinationLocationId)
                .OnDelete(DeleteBehavior.NoAction);

            // --- Decimal Warnings Fixes ---
            modelBuilder.Entity<Route>(entity =>
            {
                entity.Property(r => r.EstimatedDurationHours)
                      .HasColumnType("decimal(5, 2)");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.Property(s => s.BaseFare)
                      .HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.Property(t => t.FarePaid)
                      .HasColumnType("decimal(10, 2)");
            });
        }
    }
}