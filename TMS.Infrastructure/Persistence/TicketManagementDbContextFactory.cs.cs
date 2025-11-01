using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TMS.Infrastructure.Persistence
{
    /// <summary>
    /// Factory for creating the TicketManagementDbContext at design time (for 'dotnet ef' commands).
    /// This bypasses the need for the host application's service provider to configure DbContextOptions.
    /// </summary>
    public class TicketManagementDbContextFactory : IDesignTimeDbContextFactory<TicketManagementDbContext>
    {
        public TicketManagementDbContext CreateDbContext(string[] args)
        {
            // 1. Build configuration (to potentially load connection string from appsettings.json if needed)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                // Assuming appsettings.json exists in your startup project or the infrastructure project
                // If the appsettings.json is in a different project (e.g., TMS.Api), you might need 
                // to adjust the BasePath or reference the startup project in the ef command.
                // For simplicity, we can use a hardcoded or simple connection in this factory.
                // .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // 2. Configure DbContext Options
            var optionsBuilder = new DbContextOptionsBuilder<TicketManagementDbContext>();

            // IMPORTANT: You must provide a valid connection string here. 
            // It doesn't have to be the one used in production, but EF needs it to reflect the database provider.
            // Replace "YourDefaultConnectionString" with a string that EF Core can use 
            // to determine the database provider (e.g., SQL Server, SQLite).
            // A simple placeholder connection string is often enough for migrations to run.
            // Example for SQL Server:
            string connectionString = configuration.GetConnectionString("DefaultConnection")
                                      ?? "Server=(localdb)\\mssqllocaldb;Database=TMS_Design;Trusted_Connection=True;MultipleActiveResultSets=true";


            optionsBuilder.UseSqlServer(connectionString);

            // 3. Create and return the context instance
            return new TicketManagementDbContext(optionsBuilder.Options);
        }
    }
}
