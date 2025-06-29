using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Domain.Entities;

namespace TMS.Infrastructure.Persistence.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations"); // Specify table name

            builder.HasKey(l => l.Id); // Primary key

            builder.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(l => l.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(l => l.Address)
                .HasMaxLength(500);

            builder.Property(l => l.Latitude);
            builder.Property(l => l.Longitude);

            builder.Property(l => l.CreatedAt)
                .IsRequired();

            builder.Property(l => l.UpdatedAt)
                .IsRequired(false);
        }
    }
}