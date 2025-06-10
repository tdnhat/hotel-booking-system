using HotelBookingSystem.BookingService.Api.Endpoints;
using HotelBookingSystem.BookingService.Api.Extensions;
using HotelBookingSystem.BookingService.Application;
using HotelBookingSystem.BookingService.Infrastructure;
using HotelBookingSystem.BookingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Booking Service...");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    });

    // Add service defaults (.NET Aspire)
    builder.AddServiceDefaults();

    // Add services to the container
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "Booking Service API",
            Description = "An ASP.NET Core Web API for managing Hotel Bookings with Saga Pattern"
        });
    });

    // Add Infrastructure services (includes DbContext)
    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    // Add health checks for dependencies
    builder.Services.AddDatabaseHealthCheck(builder.Configuration, "bookingdb");

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.MapBookingEndpoints();

    // Map default endpoints (includes health checks)
    app.MapDefaultEndpoints();

    // Auto-migrate database in development
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
        await context.Database.MigrateAsync();
    }

    Log.Information("Booking Service started successfully");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Booking Service failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}