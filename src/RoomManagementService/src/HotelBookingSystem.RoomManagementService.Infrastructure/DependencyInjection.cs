using HotelBookingSystem.RoomManagementService.Application.Common.Abstractions;
using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using HotelBookingSystem.RoomManagementService.Infrastructure.Data;
using HotelBookingSystem.RoomManagementService.Infrastructure.Repositories;
using HotelBookingSystem.RoomManagementService.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBookingSystem.RoomManagementService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<RoomManagementDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("roommanagementdb")));

            // Register DbContext interface
            services.AddScoped<IRoomManagementDbContext>(provider =>
                provider.GetRequiredService<RoomManagementDbContext>());

            // Register repositories
            services.AddScoped<IHotelRepository, HotelRepository>();

            // Register services
            services.AddScoped<IDateTime, DateTimeService>();

            // Register MassTransit
            services.AddMassTransit(config =>
            {
                // Configure RabbitMQ with Aspire connection string
                config.UsingRabbitMq((context, cfg) =>
                {
                    // Use Aspire connection string if available, otherwise fallback to configuration
                    var connectionString = configuration.GetConnectionString("rabbitmq");
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        cfg.Host(connectionString);
                    }
                    else
                    {
                        // Fallback to traditional configuration for development without Aspire
                        var rabbitConfig = configuration.GetSection("RabbitMQ");
                        cfg.Host(rabbitConfig["Host"], h =>
                        {
                            h.Username(rabbitConfig["Username"]);
                            h.Password(rabbitConfig["Password"]);
                        });
                    }

                    // Configure endpoints
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}