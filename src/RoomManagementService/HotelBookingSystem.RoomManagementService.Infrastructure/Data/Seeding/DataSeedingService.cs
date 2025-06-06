using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Data.Seeding
{
    public class DataSeedingService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataSeedingService> _logger;

        public DataSeedingService(IServiceProvider serviceProvider, ILogger<DataSeedingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var seedingCoordinator = scope.ServiceProvider.GetRequiredService<DatabaseSeedingCoordinator>();

                _logger.LogInformation("Starting data seeding for {ServiceName}...", nameof(DataSeedingService));
                var startTime = DateTime.UtcNow;

                await seedingCoordinator.SeedAsync(unitOfWork, cancellationToken);

                var duration = DateTime.UtcNow - startTime;
                _logger.LogInformation("Data seeding completed in {Duration} seconds.", duration.TotalSeconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding data.");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Data seeding service is stopping.");
            return Task.CompletedTask;
        }
    }
} 