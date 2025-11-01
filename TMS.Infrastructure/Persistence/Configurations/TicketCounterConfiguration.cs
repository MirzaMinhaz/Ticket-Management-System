using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Domain.Entities;

namespace TMS.Infrastructure.Persistence.Configurations
{
    public class TicketCounterConfiguration : IEntityTypeConfiguration<TicketCounter>
    {
        public void Configure(EntityTypeBuilder<TicketCounter> builder)
        {
            // Primary Key handled by DbContext convention for BaseEntity.Id -> TicketCounterId

            builder.Property(tc => tc.LocationCode)
                .IsRequired();

            builder.Property(tc => tc.CounterName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(tc => tc.CounterCode)
                .HasMaxLength(50);

            builder.HasIndex(tc => new { tc.LocationCode, tc.CounterName })
                .IsUnique(); // Ensure unique counter name per location


            builder.Property(tc => tc.AddressDetails)
                .HasMaxLength(500);

            builder.Property(tc => tc.ContactNumber)
                .HasMaxLength(50);

            builder.Property(tc => tc.OperatingHours)
                .HasMaxLength(255);

            //builder.Property(tc => tc.Latitude)
            //    .HasColumnType("DECIMAL(9,6)");

            //builder.Property(tc => tc.Longitude)
            //    .HasColumnType("DECIMAL(9,6)");

            builder.Property(tc => tc.IsActive)
                .IsRequired();

            // Foreign Key Relationship (defined in DbContext for clarity)
            // builder.HasOne(tc => tc.Location)
            //     .WithMany(l => l.TicketCounters)
            //     .HasForeignKey(tc => tc.LocationId); // Done in DbContext

            // Relationships to Tickets (defined in DbContext)
            // builder.HasMany(tc => tc.BookingTickets)
            //     .WithOne(t => t.BookingCounter)
            //     .HasForeignKey(t => t.BookingCounterId)
            //     .IsRequired(false);

            // ... similarly for DepartureTickets and ArrivalTickets
        }
    }
}