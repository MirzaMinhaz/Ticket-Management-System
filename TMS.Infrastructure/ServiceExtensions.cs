// TMS.Infrastructure/ServiceExtensions.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TMS.Application.Interfaces;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Repositories;
using TMS.Application.Interfaces.Services;
using TMS.Application.Services;
using TMS.Infrastructure.Persistence;
using TMS.Infrastructure.Persistence.Repositories;
using TMS.Infrastructure.Repositories;

namespace TMS.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure DbContext
            services.AddDbContext<TicketManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register IGenericRepository and GenericRepository as open generics
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            // ***************************************************************
            // ***************************************************************
            // Register specific repositories
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ITicketCounterRepository, TicketCounterRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>(); // Add this if you have UnitOfWork
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IOperatorRepository, OperatorRepository>();
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<ISeatRepository, SeatRepository>();
            services.AddScoped<ITripRepository, TripRepository>();



            // Add other specific repositories as needed:
            // services.AddScoped<ITicketCounterRepository, TicketCounterRepository>();
            // services.AddScoped<IRouteRepository, RouteRepository>();
            // services.AddScoped<IScheduleRepository, ScheduleRepository>();
            // services.AddScoped<ITicketBookingRepository, TicketBookingRepository>();
            // services.AddScoped<IUserRepository, UserRepository>();


            // === NEW: Register IUnitOfWork and its implementation ===
            // This is the missing piece!
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // =======================================================
        }
    }
}