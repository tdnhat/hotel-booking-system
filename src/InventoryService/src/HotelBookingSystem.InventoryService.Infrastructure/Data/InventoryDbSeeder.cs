using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.InventoryService.Infrastructure.Data;

public static class InventoryDbSeeder
{    public static async Task SeedAsync(InventoryDbContext context, ILogger logger)
    {
        try
        {
            logger.LogInformation("Starting inventory database seeding...");

            // Check if we already have inventory data
            var existingCount = await context.RoomInventories.CountAsync();
            if (existingCount > 0)
            {
                logger.LogInformation("Inventory data already exists ({ExistingCount} records), skipping seeding", existingCount);
                return;
            }

            // Sample hotel and room type IDs (these should match what's used in other services)
            var hotelId = HotelId.From(Guid.Parse("b74bc71b-edce-4905-823f-a6bc359dfb79"));
            var roomTypeId = RoomTypeId.From(Guid.Parse("a03fb2c9-2068-445d-aba8-4bf417a32923"));

            logger.LogInformation("Seeding inventory for Hotel: {HotelId}, RoomType: {RoomTypeId}", hotelId.Value, roomTypeId.Value);            // Create room inventory for the next 365 days
            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(365);

            logger.LogInformation("Adding {Count} room inventory records to database...", (endDate - startDate).Days);
            
            // Add entities in batches to avoid tracking issues
            var batchSize = 50;
            var totalDays = (endDate - startDate).Days;
            
            for (int batch = 0; batch < totalDays; batch += batchSize)
            {
                var batchEnd = Math.Min(batch + batchSize, totalDays);
                var batchItems = new List<RoomInventory>();
                
                for (int i = batch; i < batchEnd; i++)
                {
                    var date = startDate.AddDays(i);
                    var inventoryId = InventoryId.From(Guid.NewGuid());
                    var basePrice = new Money(150.00m, Currency.USD); // $150 per night

                    var roomInventory = new RoomInventory(
                        inventoryId,
                        hotelId,
                        roomTypeId,
                        date,
                        totalRooms: 20, // 20 rooms available per day
                        basePrice);

                    batchItems.Add(roomInventory);
                }
                
                await context.RoomInventories.AddRangeAsync(batchItems);
                await context.SaveChangesAsync();
                
                // Clear tracking to avoid conflicts
                context.ChangeTracker.Clear();
                
                logger.LogInformation("Seeded batch {BatchStart}-{BatchEnd} of {Total} records", 
                    batch + 1, batchEnd, totalDays);
            }            logger.LogInformation("Successfully seeded {Count} room inventory records for hotel {HotelId} and room type {RoomTypeId}", 
                totalDays, hotelId.Value, roomTypeId.Value);

            // Add more hotels/room types if needed
            await SeedAdditionalInventoryAsync(context, logger);

            logger.LogInformation("Inventory database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while seeding inventory database");
            throw;
        }
    }    private static async Task SeedAdditionalInventoryAsync(InventoryDbContext context, ILogger logger)
    {        // Add more sample data for other hotels/room types
        var additionalHotels = new[]
        {
            new { HotelId = Guid.Parse("c85cd82c-fded-4a06-934f-b7cd460efe8a"), RoomTypeId = Guid.Parse("b14fe3da-3179-456e-bc99-5cf518b53a24"), TotalRooms = 15, BasePrice = 200.00m },
            new { HotelId = Guid.Parse("d96de93d-0eef-5b17-a45f-c8de571faf9b"), RoomTypeId = Guid.Parse("c25af4eb-4280-567f-cd0a-6da629c64b35"), TotalRooms = 25, BasePrice = 120.00m }
        };

        var startDate = DateTime.Today;
        var endDate = startDate.AddDays(365);

        foreach (var hotel in additionalHotels)
        {
            var hotelId = HotelId.From(hotel.HotelId);
            var roomTypeId = RoomTypeId.From(hotel.RoomTypeId);

            logger.LogInformation("Seeding additional inventory for Hotel: {HotelId}, RoomType: {RoomTypeId}", hotelId.Value, roomTypeId.Value);

            // Use batching for additional hotels too
            var batchSize = 50;
            var totalDays = (endDate - startDate).Days;
            
            for (int batch = 0; batch < totalDays; batch += batchSize)
            {
                var batchEnd = Math.Min(batch + batchSize, totalDays);
                var batchItems = new List<RoomInventory>();
                
                for (int i = batch; i < batchEnd; i++)
                {
                    var date = startDate.AddDays(i);
                    var inventoryId = InventoryId.From(Guid.NewGuid());
                    var basePrice = new Money(hotel.BasePrice, Currency.USD);

                    var roomInventory = new RoomInventory(
                        inventoryId,
                        hotelId,
                        roomTypeId,
                        date,
                        hotel.TotalRooms,
                        basePrice);

                    batchItems.Add(roomInventory);
                }
                
                await context.RoomInventories.AddRangeAsync(batchItems);
                await context.SaveChangesAsync();
                
                // Clear tracking to avoid conflicts
                context.ChangeTracker.Clear();
            }
        }

        logger.LogInformation("Successfully seeded additional inventory data for {Count} hotels", additionalHotels.Length);
    }
}
