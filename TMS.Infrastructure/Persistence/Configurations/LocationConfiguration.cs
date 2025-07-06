using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Domain.Entities;

namespace TMS.Infrastructure.Persistence.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            // Primary Key defined in BaseEntity, column name handled by DbContext convention
            // builder.HasKey(l => l.Id); // Already handled by BaseEntity

            // Property configurations
            builder.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(l => l.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(l => l.Address)
                .HasMaxLength(500); // Optional

            //builder.Property(l => l.Latitude)
            //    .HasColumnType("DECIMAL(9,6)"); // Specific column type for precision

            //builder.Property(l => l.Longitude)
            //    .HasColumnType("DECIMAL(9,6)"); // Specific column type for precision

            // Relationships (defined in DbContext for clarity)
            // builder.HasMany(l => l.TicketCounters)
            //     .WithOne(tc => tc.Location)
            //     .HasForeignKey(tc => tc.LocationId); // Done in DbContext
        }
    }
}