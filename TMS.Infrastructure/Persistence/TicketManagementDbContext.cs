using Microsoft.EntityFrameworkCore;
using TMS.Domain.Entities;
using TMS.Application.Interfaces.Persistence; // For IUnitOfWork
using TMS.Infrastructure.Persistence.Configurations; // For LocationConfiguration

namespace TMS.Infrastructure.Persistence
{
    public class TicketManagementDbContext : DbContext, IUnitOfWork
    {
        public TicketManagementDbContext(DbContextOptions<TicketManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<Location> Locations { get; set; }
        // Add other DbSets for Users, Operators, Vehicles, Schedules, etc.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply configurations for each entity
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            // modelBuilder.ApplyConfiguration(new UserConfiguration());
            // etc.

            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> SaveChangeAsync()
        {
            return await base.SaveChangesAsync();
        }

        public void Dispose()
        {
            // Dispose of the DbContext
            base.Dispose();
        }
    }
}