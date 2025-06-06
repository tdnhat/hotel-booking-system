using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using HotelBookingSystem.RoomManagementService.Infrastructure.Data;
using HotelBookingSystem.RoomManagementService.Infrastructure.Data.Seeding;
using HotelBookingSystem.RoomManagementService.Infrastructure.Repositories;
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
            services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register individual data seeders as IDataSeeder implementations
            services.AddScoped<IDataSeeder, RoomTypeSeeder>();
            services.AddScoped<IDataSeeder, RoomSeeder>();

            // Register database seeding coordinator
            services.AddScoped<DatabaseSeedingCoordinator>();

            // Register seeding service
            services.AddHostedService<DataSeedingService>();

            return services;
        }
    }
}