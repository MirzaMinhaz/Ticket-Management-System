using Microsoft.OpenApi.Models;
using TMS.Application; // To use AddApplicationServices extension method
using TMS.Infrastructure; // To use AddInfrastructureServices extension method

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Order here generally doesn't matter for registration, but conventions are helpful.
builder.Services.AddApplicationServices(); // Registers your ILocationService
builder.Services.AddInfrastructureServices(builder.Configuration); // Registers DbContext, Repositories

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ticket Management System API", Version = "v1" });
});

// Configure CORS for frontend access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000") // This MUST be your Next.js app's development URL
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()); // Allow cookies, auth headers etc.
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TMS.WebAPI v1"));
}

app.UseHttpsRedirection();

// Use the CORS policy - must be before UseAuthorization and MapControllers
app.UseCors("AllowSpecificOrigin");

app.UseAuthorization(); // If you implement authentication later, this is important

app.MapControllers();

app.Run();