// TMS.Application/ServiceExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using TMS.Application.Interfaces.Services; // Ensure this is correct for your app services
using TMS.Application.Services; // Ensure this is correct for your app services concrete classes
using AutoMapper;
using System.Reflection;
using TMS.Application.Interfaces;
// REMOVE: using TMS.Infrastructure.Interfaces;
// REMOVE: using TMS.Infrastructure.Repositories;

namespace TMS.Application
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // ***************************************************************
            // ***************************************************************
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ITicketCounterService, TicketCounterService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IOperatorService, OperatorService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ITripService, TripService>();
            services.AddSingleton<ISeatLockService, SeatLockService>();
            services.AddSingleton<ILoginAttemptTracker, LoginAttemptTracker>();
            return services;
        }

        // REMOVE THE ENTIRE METHOD BELOW
        /*
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // This code will cause errors because TMS.Application no longer references TMS.Infrastructure
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ITicketCounterRepository, TicketCounterRepository>();
            return services;
        }
        */
    }
}