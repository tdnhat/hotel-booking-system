using HotelBookingSystem.RoomManagementService.Api.Extensions;
using HotelBookingSystem.RoomManagementService.Application;
using HotelBookingSystem.RoomManagementService.Infrastructure;
using HotelBookingSystem.RoomManagementService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
            Description = "An ASP.NET Core Web API for managing Rooms and their Availability"
        });
    });

    // Add Infrastructure services
    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.MapRoomManagementEndpoints();

    // Auto-migrate database in development
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RoomManagementDbContext>();
        await context.Database.MigrateAsync();
    }

    Log.Information("Room Management Service started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
