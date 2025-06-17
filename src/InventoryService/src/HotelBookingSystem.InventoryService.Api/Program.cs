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

    // Add Application and Infrastructure services
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
    app.MapDefaultEndpoints();    // Auto-migrate database in development
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        
        // Retry logic for database migrations
        var maxRetries = 5;
        var delay = TimeSpan.FromSeconds(2);
        
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                Log.Information("Applying database migrations... (Attempt {Attempt}/{MaxRetries})", i + 1, maxRetries);
                await context.Database.MigrateAsync();
                Log.Information("Database migrations applied successfully");
                break;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                Log.Warning(ex, "Failed to apply migrations on attempt {Attempt}/{MaxRetries}. Retrying in {Delay} seconds...", i + 1, maxRetries, delay.TotalSeconds);
                await Task.Delay(delay);
                delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Exponential backoff
            }
        }        // Seed initial inventory data
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        await InventoryDbSeeder.SeedAsync(context, logger);
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