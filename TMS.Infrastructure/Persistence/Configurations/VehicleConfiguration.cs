using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Domain.Entities;

namespace TMS.Infrastructure.Persistence.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            // Table name
            builder.ToTable("Vehicles");

            // Configure primary key
            builder.HasKey(v => v.Id);

            // Configure properties
            builder.Property(v => v.OperatorCode).IsRequired(false); // ✅ Make nullable
            builder.Property(v => v.Type).HasMaxLength(50).IsRequired();
            builder.Property(v => v.Model).HasMaxLength(100);
            builder.Property(v => v.LicensePlate).HasMaxLength(20).IsRequired();
            builder.Property(v => v.Capacity).IsRequired();
            builder.Property(v => v.VehicleCode).HasMaxLength(20);
            builder.Property(v => v.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(v => v.LastModifiedAt).IsRequired(false);

            // Configure relationship with Operator
            builder.HasOne(v => v.Operator)
                   .WithMany()
                   .HasForeignKey(v => v.OperatorCode)
                   .OnDelete(DeleteBehavior.Restrict); // Use Restrict to prevent cascade delete

            // Add a unique index for the VehicleCode
            builder.HasIndex(v => v.VehicleCode).IsUnique();
        }
    }
}
