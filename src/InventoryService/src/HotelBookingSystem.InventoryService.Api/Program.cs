using HotelBookingSystem.InventoryService.Api.Extensions;
using HotelBookingSystem.InventoryService.Application;
using HotelBookingSystem.InventoryService.Infrastructure;
using HotelBookingSystem.InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Inventory Service...");

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
            Title = "Inventory Service API",
            Description = "An ASP.NET Core Web API for managing Hotel Room Inventory"
        });
    });

    // Add application and infrastructure services
    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    // Add health checks for dependencies
    builder.Services.AddDatabaseHealthCheck(builder.Configuration, "inventorydb");

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.MapInventoryEndpoints();

    // Map default endpoints (includes health checks)
    app.MapDefaultEndpoints();

    // Auto-migrate database in development (only if database is available)
    if (app.Environment.IsDevelopment())
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            await context.Database.MigrateAsync();
            Log.Information("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Could not apply database migrations. Database may not be available yet. Service will continue to start.");
        }
    }

    Log.Information("Inventory Service started successfully");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Inventory Service failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
