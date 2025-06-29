using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TMS.Application.Interfaces.Persistence;
using TMS.Infrastructure.Persistence;
using TMS.Infrastructure.Persistence.Repositories;

namespace TMS.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure DbContext with SQL Server
            services.AddDbContext<TicketManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(TicketManagementDbContext).Assembly.FullName))); // Specify migrations assembly

            // Register repositories and Unit of Work
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IUnitOfWork, TicketManagementDbContext>(); // DbContext itself acts as UnitOfWork for SaveChanges
        }
    }
}