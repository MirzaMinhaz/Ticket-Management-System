// TMS.Application/ServiceExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using System.Reflection; // Required for Assembly.GetExecutingAssembly()
using AutoMapper; // Required for AddAutoMapper

// Required for ILocationService
using TMS.Application.Interfaces.Services; // <--- This line is CRUCIAL for ILocationService

// Required for LocationService
using TMS.Application.Services; // <--- This line is CRUCIAL for LocationService

namespace TMS.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register your other services here
            // This is line 18, where the errors are reported
            services.AddScoped<ILocationService, LocationService>();

            // ... potentially other service registrations
        }
    }
}