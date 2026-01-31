using Microsoft.EntityFrameworkCore;
using TMS.Domain.Entities;
using System;

namespace TMS.Infrastructure.Persistence
{
    public class TicketManagementDbContext : DbContext
    {
        public TicketManagementDbContext(DbContextOptions<TicketManagementDbContext> options)
            : base(options)
        {
        }

        // Define DbSets for all your entities
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketCounter> TicketCounters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Global Decimal Precision Configuration (Optional but good practice) ---
            modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 128); // For SQL Server
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetColumnType("decimal(18,2)");
                    }
                }
            }

            // --- Configure Location entity ---
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LocationCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.LocationCode).IsUnique();

                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Address).HasMaxLength(250);

                entity.HasIndex(e => new { e.Name, e.Type }).IsUnique();

                // Navigation properties (Collections on Location)
                entity.HasMany(l => l.DepartureRoutes)
                      .WithOne(r => r.DepartureLocation)
                      .HasForeignKey(r => r.DepartureLocationCode)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(l => l.DestinationRoutes)
                      .WithOne(r => r.DestinationLocation)
                      .HasForeignKey(r => r.DestinationLocationCode)
                      .OnDelete(DeleteBehavior.Restrict);

                // ✅ FIX: Explicitly configure the inverse relationship from Location to TicketCounter
                // This forces the collection to use the string foreign key (LocationCode)
                // and prevents the convention from creating the LocationId shadow property.
                entity.HasMany(l => l.TicketCounters)
                      .WithOne() // The TicketCounter entity does not have a Location Navigation property
                      .HasPrincipalKey(l => l.LocationCode) // Location's unique key is LocationCode
                      .HasForeignKey(tc => tc.LocationCode) // TicketCounter's foreign key is LocationCode
                      .IsRequired(); // Assuming LocationCode is a required FK in TicketCounter
            });

            // --- Configure TicketCounter entity ---
            modelBuilder.Entity<TicketCounter>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CounterCode)
                      .IsRequired()
                      .HasMaxLength(10);
                entity.HasIndex(e => e.CounterCode).IsUnique();

                entity.Property(e => e.CounterName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.AddressDetails).HasMaxLength(250);
                entity.Property(e => e.ContactNumber).HasMaxLength(20);
                entity.Property(e => e.OperatingHours).HasMaxLength(100);

                entity.HasIndex(tc => tc.CounterName);

                // ❌ Removed the old block:
                // entity.HasOne<Location>().WithMany().HasForeignKey(tc => tc.LocationCode).HasPrincipalKey(l => l.LocationCode).OnDelete(DeleteBehavior.Restrict);
                // This redundant configuration is now handled in the Location entity block above (entity.HasMany(l => l.TicketCounters)...)
            });


            // --- Configure Operator entity ---
            modelBuilder.Entity<Operator>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasColumnType("int")
                .UseIdentityColumn();

                entity.Property(e => e.OperatorCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.OperatorCode).IsUnique();

                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Type).HasMaxLength(50);

                entity.HasMany(o => o.Vehicles)
                      .WithOne(v => v.Operator)
                      .HasForeignKey(v => v.OperatorCode)
                      .HasPrincipalKey(o => o.OperatorCode)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Configure Role entity ---
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.RoleName).IsUnique();
            });

            // --- Configure Route entity ---
            modelBuilder.Entity<Route>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.RouteCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.RouteCode).IsUnique();

                entity.Property(e => e.RouteName).IsRequired().HasMaxLength(200);

                // Foreign keys to DepartureLocation and DestinationLocation
                entity.HasOne(r => r.DepartureLocation)
                      .WithMany(l => l.DepartureRoutes)
                      .HasForeignKey(r => r.DepartureLocationCode)
                      .HasPrincipalKey(l => l.LocationCode)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.DestinationLocation)
                      .WithMany(l => l.DestinationRoutes)
                      .HasForeignKey(r => r.DestinationLocationCode)
                      .HasPrincipalKey(l => l.LocationCode)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(r => r.Schedules)
                      .WithOne(s => s.Route)
                      .HasForeignKey(s => s.RouteId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Add indices for foreign keys and common query fields
                entity.HasIndex(r => r.DepartureLocationCode);
                entity.HasIndex(r => r.DestinationLocationCode);
                entity.HasIndex(r => new { r.DepartureLocationCode, r.DestinationLocationCode, r.RouteName }).IsUnique();
            });

            // --- Configure Schedule entity ---
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ScheduleCode).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.ScheduleCode).IsUnique();

                entity.Property(e => e.Status).HasMaxLength(50);

                // Foreign keys
                entity.HasOne(s => s.Route)
                      .WithMany(r => r.Schedules)
                      .HasForeignKey(s => s.RouteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Vehicle)
                      .WithMany(v => v.Schedules)
                      .HasForeignKey(s => s.VehicleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(s => s.Seats)
                      .WithOne(seat => seat.Schedule)
                      .HasForeignKey(seat => seat.ScheduleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(s => s.Tickets)
                      .WithOne(t => t.Schedule)
                      .HasForeignKey(t => t.ScheduleId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Add indices for foreign keys and common query fields
                entity.HasIndex(s => s.RouteId);
                entity.HasIndex(s => s.VehicleId);
                entity.HasIndex(s => s.DepartureDateTime);
                entity.HasIndex(s => s.ArrivalDateTime);
            });

            // --- Configure Seat entity ---
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.SeatCode)
                .IsRequired()
                .HasMaxLength(20);

                entity.HasIndex(e => e.SeatCode)
                .IsUnique();

                entity.Property(e => e.SeatNumber)
                .IsRequired()
                .HasMaxLength(5);

                // Foreign key to Schedule
                entity.HasOne(s => s.Schedule)
                      .WithMany(sch => sch.Seats)
                      .HasForeignKey(s => s.ScheduleId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Crucial: A seat number must be unique per schedule
                entity.HasIndex(s => new { s.ScheduleId, s.SeatNumber }).IsUnique();

                //entity.HasMany(s => s.Tickets)
                //      .WithOne(t => t.BookedSeat)
                //      .HasForeignKey(t => t.SeatId)
                //      .OnDelete(DeleteBehavior.Restrict);

                //entity.HasIndex(s => s.ScheduleId);
            });

            // --- Configure Ticket entity ---
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.TicketCode).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.TicketCode).IsUnique();

                entity.Property(e => e.PassengerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PassengerContact).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);

                // Foreign keys
                entity.HasOne(t => t.User)
                      .WithMany(u => u.Tickets)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Schedule)
                      .WithMany(s => s.Tickets)
                      .HasForeignKey(t => t.ScheduleId)
                      .OnDelete(DeleteBehavior.Restrict);

                //entity.HasOne(t => t.BookedSeat)
                //      .WithMany(s => s.Tickets)
                //      .HasForeignKey(t => t.SeatId)
                //      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(t => t.SeatCode)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(t => t.SeatNumber)
                      .IsRequired()
                      .HasMaxLength(5);

                // === Prevent Multiple Cascade Paths: Explicitly set DeleteBehavior.NoAction ===
                entity.HasOne(t => t.BookingCounter)
                      .WithMany(tc => tc.BookingTickets)
                      .HasForeignKey(t => t.BookingCounterId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(t => t.DepartureCounter)
                      .WithMany(tc => tc.DepartureTickets)
                      .HasForeignKey(t => t.DepartureCounterId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(t => t.ArrivalCounter)
                      .WithMany(tc => tc.ArrivalTickets)
                      .HasForeignKey(t => t.ArrivalCounterId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);
                // ===================================================================

                entity.HasMany(t => t.Comments)
                      .WithOne(c => c.Ticket)
                      .HasForeignKey(c => c.TicketId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Add indices for foreign keys and common query fields
                entity.HasIndex(t => t.UserId);
                entity.HasIndex(t => t.ScheduleId);
                //entity.HasIndex(t => t.SeatId);
                entity.HasIndex(t => t.SeatCode);
                entity.HasIndex(t => t.SeatNumber);
                entity.HasIndex(t => t.BookingDateTime);
                entity.HasIndex(t => t.Status);
            });

            // --- Configure User entity ---
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.UserCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.UserCode).IsUnique();

                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);

                entity.HasMany(u => u.Tickets)
                      .WithOne(t => t.User)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Comments)
                      .WithOne(c => c.User)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Configure Vehicle entity ---
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.VehicleCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.VehicleCode).IsUnique();

                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Model).HasMaxLength(100);
                entity.Property(e => e.LicensePlate).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.LicensePlate).IsUnique();

                entity.HasOne(v => v.Operator)
                      .WithMany(o => o.Vehicles)
                      .HasForeignKey(v => v.OperatorCode)
                      .HasPrincipalKey(o => o.OperatorCode)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(v => v.Schedules)
                      .WithOne(s => s.Vehicle)
                      .HasForeignKey(s => s.VehicleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(v => v.OperatorCode);
            });

            // --- Configure Comment entity ---
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Content).IsRequired().HasMaxLength(500);

                // Foreign keys
                entity.HasOne(c => c.Ticket)
                      .WithMany(t => t.Comments)
                      .HasForeignKey(c => c.TicketId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.TicketId);
                entity.HasIndex(c => c.UserId);
                entity.HasIndex(c => c.CreatedAt);
            });
        }
    }
}
