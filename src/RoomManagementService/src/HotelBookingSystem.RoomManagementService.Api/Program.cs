using HotelBookingSystem.RoomManagementService.Api.Extensions;
using HotelBookingSystem.RoomManagementService.Application;
using HotelBookingSystem.RoomManagementService.Infrastructure;
using HotelBookingSystem.RoomManagementService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Room Management Service...");

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
            Title = "Room Management Service API",
            Description = "An ASP.NET Core Web API for managing Hotels and Room Types"
        });
    });

    // Add Application and Infrastructure services
    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    // Add health checks for dependencies
    builder.Services.AddDatabaseHealthCheck(builder.Configuration, "roommanagementdb");

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.MapRoomManagementEndpoints();

    // Map default endpoints (includes health checks)
    app.MapDefaultEndpoints();

    // Auto-migrate database in development
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RoomManagementDbContext>();
        await context.Database.MigrateAsync();
    }

    Log.Information("Room Management Service started successfully");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Room Management Service failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
