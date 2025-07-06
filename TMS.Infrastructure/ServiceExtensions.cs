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
            services.AddDbContext<TicketManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register specific repositories
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ITicketCounterRepository, TicketCounterRepository>(); // NEW
            // Add other specific repositories here
        }
    }
}