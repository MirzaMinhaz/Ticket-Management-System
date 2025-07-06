using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TMS.Application.Interfaces.Services;
using TMS.Application.Services;

namespace TMS.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register Application Services
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ITicketCounterService, TicketCounterService>(); // NEW
            // Add other application services here
        }
    }
}