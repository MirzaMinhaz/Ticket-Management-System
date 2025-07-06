using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection; // For ApplyConfigurationsFromAssembly
using TMS.Domain.Entities;

namespace TMS.Infrastructure.Persistence
{
    public class TicketManagementDbContext : DbContext
    {
        public TicketManagementDbContext(DbContextOptions<TicketManagementDbContext> options)
            : base(options)
        {
        }

        // Define your DbSets here
        public DbSet<Location> Locations { get; set; }
        public DbSet<TicketCounter> TicketCounters { get; set; } // NEW DbSet
        public DbSet<User> Users { get; set; }
        // Add other DbSets for your entities: Operators, Vehicles, Routes, Schedules, Seats, Tickets, Comments

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all configurations from the current assembly (where your EntityTypeConfiguration classes are)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // --- Custom Convention for Primary Key Column Names (OPTIONAL, but useful) ---
            // This convention automatically renames 'Id' property in BaseEntity to 'EntityNameId' column in DB
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // Find the primary key property (assumed to be 'Id' from BaseEntity)
                    var idProperty = entityType.FindProperty("Id");
                    if (idProperty != null)
                    {
                        // Set the column name to "EntityNameId"
                        idProperty.SetColumnName(entityType.DisplayName() + "Id");
                    }
                }
            }

            // Define specific relationships if not handled by convention or attributes
            // Location and TicketCounter One-to-Many
            modelBuilder.Entity<Location>()
                .HasMany(l => l.TicketCounters)
                .WithOne(tc => tc.Location)
                .HasForeignKey(tc => tc.LocationId);

            // Ticket and TicketCounter relationships (NEW)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.BookingCounter)
                .WithMany(tc => tc.BookingTickets)
                .HasForeignKey(t => t.BookingCounterId)
                .IsRequired(false); // Nullable FK

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

            base.OnModelCreating(modelBuilder);
        }
    }
}