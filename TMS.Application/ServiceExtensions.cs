using Microsoft.Extensions.DependencyInjection;
using System.Reflection; // For Assembly.GetExecutingAssembly()
using TMS.Application.Interfaces.Services; // Add this using
using TMS.Application.Services;       // Add this using

namespace TMS.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register your Application Services
            services.AddScoped<ILocationService, LocationService>();

            // You would register other application services here as well
            // services.AddScoped<IOperatorService, OperatorService>();
        }
    }
}