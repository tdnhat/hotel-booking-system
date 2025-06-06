using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Data.Seeding
{
    public class RoomSeeder : IDataSeeder
    {
        private readonly ILogger<RoomSeeder> _logger;

        public RoomSeeder(ILogger<RoomSeeder> logger)
        {
            _logger = logger;
        }

        public async Task SeedAsync(IUnitOfWork unitOfWork, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Room seeding skipped - will be implemented when Room repository is available.");
            // TODO: Implement room seeding when IRoomRepository is created
            await Task.CompletedTask;
        }
    }
}