// TMS.WebAPI/Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Ensure this is present
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TMS.Application;
using TMS.Application;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Application.Services;
using TMS.Infrastructure;
using TMS.Infrastructure.Persistence;
using TMS.Infrastructure.Persistence;
using TMS.Infrastructure.Persistence.Repositories;
using TMS.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// ***************************************************************
// Crucial: Register your custom services and repositories here
builder.Services.AddApplicationServices();   // <-- This line registers ITicketCounterService

// ***************************************************************


// IMPORTANT: Register your DbContext here!
builder.Services.AddDbContext<TicketManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // REPLACE "YourDbContext" and connection string

// Add your custom service extensions for Application and Infrastructure layers
// This line will call the AddInfrastructureServices method that registers your DbContext
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// --- START: NEW/UPDATED SERVICE REGISTRATIONS ---

// Register IHttpContextAccessor. This is crucial for your DbContext to get the current user context.
builder.Services.AddHttpContextAccessor(); // <--- ADD OR ENSURE THIS LINE IS PRESENT

// Configure CORS (Cross-Origin Resource Sharing)
// This is essential for your Angular frontend to talk to your backend API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder => policyBuilder.WithOrigins("http://localhost:4200") // Your Angular app URL
                                     .AllowAnyHeader()
                                     .AllowAnyMethod());
});
// --- END: NEW/UPDATED SERVICE REGISTRATIONS ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS policy
app.UseCors("AllowSpecificOrigin");

// Ensure UseAuthorization is after UseCors
app.UseAuthorization();

app.MapControllers();

app.Run();