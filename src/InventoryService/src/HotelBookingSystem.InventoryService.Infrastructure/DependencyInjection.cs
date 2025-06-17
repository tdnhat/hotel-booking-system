using HotelBookingSystem.InventoryService.Application.Interfaces;
using HotelBookingSystem.InventoryService.Domain.Repositories;
using HotelBookingSystem.InventoryService.Domain.Services;
using HotelBookingSystem.InventoryService.Infrastructure.Consumers;
using HotelBookingSystem.InventoryService.Infrastructure.Data;
using HotelBookingSystem.InventoryService.Infrastructure.Repositories;
using HotelBookingSystem.InventoryService.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBookingSystem.InventoryService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Entity Framework
            services.AddDbContext<InventoryDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("inventorydb")));

            // Register DbContext interface
            services.AddScoped<Application.Interfaces.IInventoryDbContext>(provider =>
                provider.GetRequiredService<InventoryDbContext>());

            // Add repositories
            services.AddScoped<Domain.Repositories.IRoomInventoryRepository, RoomInventoryRepository>();
            services.AddScoped<Domain.Repositories.IRoomHoldRepository, RoomHoldRepository>();

            // Add domain services
            services.AddScoped<IInventoryDomainService, InventoryDomainService>();

            // Add external services
            services.AddScoped<IDateTime, DateTimeService>();

            // Add MassTransit
            services.AddMassTransit(config =>
            {
                // Add consumers
                config.AddConsumer<HoldRoomConsumer>();
                config.AddConsumer<ReleaseRoomConsumer>();

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

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }

    // External service implementations
    public interface IDateTime
    {
        DateTime UtcNow { get; }
        DateTime Now { get; }
    }

    public class DateTimeService : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Now => DateTime.Now;
    }
}