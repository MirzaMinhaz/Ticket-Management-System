using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TMS.Infrastructure.Persistence
{
    public class TicketManagementDbContextFactory
        : IDesignTimeDbContextFactory<TicketManagementDbContext>
    {
        public TicketManagementDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "TMS.WebAPI"
            );

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(
                    "appsettings.json",
                    optional: false,
                    reloadOnChange: false
                )
                .AddJsonFile(
                    "appsettings.Development.json",
                    optional: true,
                    reloadOnChange: false
                )
                .Build();

            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "DefaultConnection was not found in TMS.WebAPI appsettings.json."
                );
            }

            var optionsBuilder =
                new DbContextOptionsBuilder<TicketManagementDbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new TicketManagementDbContext(optionsBuilder.Options);
        }
    }
}