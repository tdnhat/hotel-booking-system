using HotelBookingSystem.BookingService.Domain.Repositories;
using HotelBookingSystem.BookingService.Infrastructure.Data;
using HotelBookingSystem.BookingService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using HotelBookingSystem.BookingService.Application.Interfaces;
using MassTransit;
using HotelBookingSystem.BookingService.Infrastructure.Saga;
using HotelBookingSystem.BookingService.Domain.Entities;
using HotelBookingSystem.BookingService.Infrastructure.Consumers;

namespace HotelBookingSystem.BookingService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<BookingDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("bookingdb")));

            // Register DbContext interface
            services.AddScoped<IBookingDbContext>(provider =>
                provider.GetRequiredService<BookingDbContext>());

            // Register repositories
            services.AddScoped<ISagaRepository, SagaRepository>();

            // Register MassTransit with Saga
            services.AddMassTransit(config =>
            {
                config.AddSagaStateMachine<BookingSagaStateMachine, BookingState>(typeof(BookingSagaDefinition))
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Optimistic; // Handle concurrency access
                        r.ExistingDbContext<BookingDbContext>(); // Use the same DbContext for the repository
                    });

                config.AddConsumer<ProcessPaymentConsumer>();

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

                    // Configure endpoints for our saga
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}