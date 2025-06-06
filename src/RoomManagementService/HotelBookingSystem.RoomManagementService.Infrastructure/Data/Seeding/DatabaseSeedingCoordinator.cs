using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Data.Seeding
{
    public class DatabaseSeedingCoordinator
    {
        private readonly ILogger<DatabaseSeedingCoordinator> _logger;
        private readonly IEnumerable<IDataSeeder> _seeders;

        public DatabaseSeedingCoordinator(ILogger<DatabaseSeedingCoordinator> logger, IEnumerable<IDataSeeder> seeders)
        {
            _logger = logger;
            _seeders = seeders;
        }

        public async Task SeedAsync(IUnitOfWork unitOfWork, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting database seeding...");

            foreach (var seeder in _seeders)
            {
                var seederName = seeder.GetType().Name;
                _logger.LogInformation("Running seeder: {SeederName}", seederName);
                
                try
                {
                    await seeder.SeedAsync(unitOfWork, cancellationToken);
                    _logger.LogInformation("Completed seeder: {SeederName}", seederName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in seeder: {SeederName}", seederName);
                    throw;
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Database seeding completed successfully.");
        }
    }
}