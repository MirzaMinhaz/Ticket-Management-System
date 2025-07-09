// TMS.Infrastructure/Persistence/TicketManagementDbContext.cs
using Microsoft.EntityFrameworkCore;
using TMS.Domain.Entities;
using System; // Required for DateTime

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
            // This applies a default precision/scale to all decimal properties unless overridden
            modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 128); // For SQL Server
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetColumnType("decimal(18,2)"); // Default precision and scale for decimals
                    }
                }
            }

            // --- Configure Location entity ---
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LocationCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.LocationCode).IsUnique(); // Ensure LOC-0001 is unique

                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Address).HasMaxLength(250);

                // Combined unique index for Name and Type
                entity.HasIndex(e => new { e.Name, e.Type }).IsUnique();

                // Navigation properties (Collections on Location)
                entity.HasMany(l => l.TicketCounters)
                      .WithOne(tc => tc.Location)
                      .HasForeignKey(tc => tc.LocationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(l => l.DepartureRoutes)
                      .WithOne(r => r.DepartureLocation)
                      .HasForeignKey(r => r.DepartureLocationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(l => l.DestinationRoutes)
                      .WithOne(r => r.DestinationLocation)
                      .HasForeignKey(r => r.DestinationLocationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Configure TicketCounter entity ---
            modelBuilder.Entity<TicketCounter>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CounterCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.CounterCode).IsUnique(); // Ensure TCO-0001 is unique

                entity.Property(e => e.CounterName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AddressDetails).HasMaxLength(250);
                entity.Property(e => e.ContactNumber).HasMaxLength(20);
                entity.Property(e => e.OperatingHours).HasMaxLength(100);

                // Foreign key to Location
                entity.HasOne(tc => tc.Location)
                      .WithMany(l => l.TicketCounters)
                      .HasForeignKey(tc => tc.LocationId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Add indices for common lookups
                entity.HasIndex(tc => tc.LocationId);
                entity.HasIndex(tc => tc.CounterName);
            });

            // --- Configure Operator entity ---
            modelBuilder.Entity<Operator>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.OperatorCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.OperatorCode).IsUnique(); // Ensure OPR-0001 is unique

                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name).IsUnique(); // Operator name should be unique
                entity.Property(e => e.Type).HasMaxLength(50);

                entity.HasMany(o => o.Vehicles)
                      .WithOne(v => v.Operator)
                      .HasForeignKey(v => v.OperatorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Configure Role entity ---
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.RoleName).IsUnique(); // Role names should be unique
            });

            // --- Configure Route entity ---
            modelBuilder.Entity<Route>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.RouteCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.RouteCode).IsUnique(); // Ensure RTE-0001 is unique

                entity.Property(e => e.RouteName).IsRequired().HasMaxLength(200);
                // Decimal type configured globally, but can be overridden here if needed:
                // entity.Property(e => e.EstimatedDurationHours).HasColumnType("decimal(18,2)");

                // Foreign keys to DepartureLocation and DestinationLocation
                entity.HasOne(r => r.DepartureLocation)
                      .WithMany(l => l.DepartureRoutes)
                      .HasForeignKey(r => r.DepartureLocationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.DestinationLocation)
                      .WithMany(l => l.DestinationRoutes)
                      .HasForeignKey(r => r.DestinationLocationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(r => r.Schedules)
                      .WithOne(s => s.Route)
                      .HasForeignKey(s => s.RouteId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Add indices for foreign keys and common query fields
                entity.HasIndex(r => r.DepartureLocationId);
                entity.HasIndex(r => r.DestinationLocationId);
                // Composite unique index for routes (e.g., Dhaka to Chittagong should be unique)
                entity.HasIndex(r => new { r.DepartureLocationId, r.DestinationLocationId, r.RouteName }).IsUnique();
            });

            // --- Configure Schedule entity ---
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ScheduleCode).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.ScheduleCode).IsUnique(); // Ensure SCD-0001 is unique

                entity.Property(e => e.Status).HasMaxLength(50);
                // Decimal type configured globally:
                // entity.Property(e => e.BaseFare).HasColumnType("decimal(18,2)");

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
                      .OnDelete(DeleteBehavior.Cascade); // Seats are intrinsically linked to a schedule

                entity.HasMany(s => s.Tickets)
                      .WithOne(t => t.Schedule)
                      .HasForeignKey(t => t.ScheduleId)
                      .OnDelete(DeleteBehavior.Restrict); // Tickets should not be deleted if schedule is deleted

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

                entity.Property(e => e.SeatCode).IsRequired().HasMaxLength(20); // e.g. SEA-0001
                entity.HasIndex(e => e.SeatCode).IsUnique(); // Ensure SEA-0001 is unique

                entity.Property(e => e.SeatNumber).IsRequired().HasMaxLength(5);

                // Foreign key to Schedule
                entity.HasOne(s => s.Schedule)
                      .WithMany(sch => sch.Seats)
                      .HasForeignKey(s => s.ScheduleId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Crucial: A seat number must be unique per schedule
                entity.HasIndex(s => new { s.ScheduleId, s.SeatNumber }).IsUnique();

                entity.HasMany(s => s.Tickets)
                      .WithOne(t => t.BookedSeat)
                      .HasForeignKey(t => t.SeatId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => s.ScheduleId);
            });

            // --- Configure Ticket entity ---
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.TicketCode).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.TicketCode).IsUnique(); // Ensure TIC-0001 is unique

                entity.Property(e => e.PassengerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PassengerContact).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);
                // Decimal type configured globally:
                // entity.Property(e => e.FarePaid).HasColumnType("decimal(18,2)");

                // Foreign keys
                entity.HasOne(t => t.User)
                      .WithMany(u => u.Tickets)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Schedule)
                      .WithMany(s => s.Tickets)
                      .HasForeignKey(t => t.ScheduleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.BookedSeat)
                      .WithMany(s => s.Tickets)
                      .HasForeignKey(t => t.SeatId)
                      .OnDelete(DeleteBehavior.Restrict);

                // === FIX FOR CASCADE PATHS: Explicitly set DeleteBehavior.NoAction ===
                // This is crucial to prevent multiple cascade paths when deleting TicketCounters
                entity.HasOne(t => t.BookingCounter)
                      .WithMany(tc => tc.BookingTickets) // Assuming collection on TicketCounter
                      .HasForeignKey(t => t.BookingCounterId)
                      .IsRequired(false) // Matches nullable FK property
                      .OnDelete(DeleteBehavior.NoAction); // Changed from SetNull to NoAction

                entity.HasOne(t => t.DepartureCounter)
                      .WithMany(tc => tc.DepartureTickets) // Assuming collection on TicketCounter
                      .HasForeignKey(t => t.DepartureCounterId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction); // Changed from SetNull to NoAction

                entity.HasOne(t => t.ArrivalCounter)
                      .WithMany(tc => tc.ArrivalTickets) // Assuming collection on TicketCounter
                      .HasForeignKey(t => t.ArrivalCounterId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction); // Changed from SetNull to NoAction
                // ===================================================================

                entity.HasMany(t => t.Comments)
                      .WithOne(c => c.Ticket)
                      .HasForeignKey(c => c.TicketId)
                      .OnDelete(DeleteBehavior.Cascade); // Comments are usually deleted with their ticket

                // Add indices for foreign keys and common query fields
                entity.HasIndex(t => t.UserId);
                entity.HasIndex(t => t.ScheduleId);
                entity.HasIndex(t => t.SeatId);
                entity.HasIndex(t => t.BookingDateTime);
                entity.HasIndex(t => t.Status);
            });

            // --- Configure User entity ---
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.UserCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.UserCode).IsUnique(); // Ensure USR-0001 is unique

                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique(); // Username should be unique
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique(); // Email should be unique

                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255); // Standard for hashed passwords
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50); // Consider FK to Role entity if you have a UserRole entity

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
                entity.HasIndex(e => e.VehicleCode).IsUnique(); // Ensure VHC-0001 is unique

                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Model).HasMaxLength(100);
                entity.Property(e => e.LicensePlate).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.LicensePlate).IsUnique(); // License plates should be unique

                entity.HasOne(v => v.Operator)
                      .WithMany(o => o.Vehicles)
                      .HasForeignKey(v => v.OperatorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(v => v.Schedules)
                      .WithOne(s => s.Vehicle)
                      .HasForeignKey(s => s.VehicleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(v => v.OperatorId);
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
                      .OnDelete(DeleteBehavior.Cascade); // Comments are dependent on Ticket

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.TicketId);
                entity.HasIndex(c => c.UserId);
                entity.HasIndex(c => c.CreatedAt);
            });

            // --- Global Audit Property Configuration (CreatedAt, LastModifiedAt, CreatedBy, LastModifiedBy) ---
            // This is a common pattern to automatically set audit properties.
            // You might implement this in SaveChangesAsync in DbContext or via an interceptor.
            // This section is commented out as it's typically handled outside OnModelCreating,
            // but included as a reminder of where it would fit if done via explicit property config.
            /*
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity<int>).IsAssignableFrom(entityType.ClrType)) // Check if it inherits BaseEntity<int>
                {
                    modelBuilder.Entity(entityType.Name).Property<DateTime>(nameof(BaseEntity<int>.CreatedAt)).IsRequired();
                    modelBuilder.Entity(entityType.Name).Property<DateTime?>(nameof(BaseEntity<int>.LastModifiedAt));
                    modelBuilder.Entity(entityType.Name).Property<string>(nameof(BaseEntity<int>.CreatedBy)).IsRequired(false); // Make nullable if not always set
                    modelBuilder.Entity(entityType.Name).Property<string>(nameof(BaseEntity<int>.LastModifiedBy)).IsRequired(false);
                }
            }
            */
        }
    }
}