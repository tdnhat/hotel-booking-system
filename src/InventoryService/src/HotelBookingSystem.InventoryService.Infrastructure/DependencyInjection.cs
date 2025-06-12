using HotelBookingSystem.InventoryService.Application.Interfaces;
using HotelBookingSystem.InventoryService.Infrastructure.Data;
using HotelBookingSystem.InventoryService.Infrastructure.Repositories;
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

            // Add repositories
            services.AddScoped<IRoomAvailabilityRepository, RoomAvailabilityRepository>();
            services.AddScoped<IRoomHoldRepository, RoomHoldRepository>();

            // Add MassTransit
            services.AddMassTransit(config =>
            {
                // Add consumers
                config.AddConsumersFromNamespaceContaining<Consumers.HoldRoomConsumer>();

                config.UsingRabbitMq((context, cfg) =>
                {
                    // Use Aspire connection string if available, otherwise fallback to configuration
                    var connectionString = configuration.GetConnectionString("messaging");
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        cfg.Host(connectionString);
                    }
                    else
                    {
                        // Fallback to traditional configuration for development without Aspire
                        cfg.Host(configuration.GetConnectionString("RabbitMQ"));
                    }

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
} 