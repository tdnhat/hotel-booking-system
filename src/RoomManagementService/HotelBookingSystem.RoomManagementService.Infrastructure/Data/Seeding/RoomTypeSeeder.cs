using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBookingSystem.RoomManagementService.Application.Queries.GetRoomTypes;
using HotelBookingSystem.RoomManagementService.Domain.Entities;
using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Data.Seeding
{
    public class RoomTypeSeeder : IDataSeeder
    {
        private readonly ILogger<RoomTypeSeeder> _logger;

        public RoomTypeSeeder(ILogger<RoomTypeSeeder> logger)
        {
            _logger = logger;
        }

        public async Task SeedAsync(IUnitOfWork unitOfWork, CancellationToken cancellationToken = default)
        {
            // Check if room types already exist
            var existingRoomTypes = await unitOfWork.RoomTypes.CountAsync(cancellationToken: cancellationToken);
            
            if (existingRoomTypes > 0)
            {
                _logger.LogInformation("Room types already exist. Skipping seeding.");
                return;
            }

            _logger.LogInformation("Seeding room types...");

            var hotelId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // Sample hotel ID

            var roomTypes = new List<RoomType>
            {
                new RoomType(
                    id: Guid.NewGuid(),
                    hotelId: hotelId,
                    name: "Standard Room",
                    description: "A comfortable standard room with basic amenities",
                    pricePerNight: 99.99m,
                    maxGuests: 2,
                    totalRooms: 10
                ),
                new RoomType(
                    id: Guid.NewGuid(),
                    hotelId: hotelId,
                    name: "Deluxe Room",
                    description: "A spacious deluxe room with premium amenities",
                    pricePerNight: 149.99m,
                    maxGuests: 3,
                    totalRooms: 5
                ),
                new RoomType(
                    id: Guid.NewGuid(),
                    hotelId: hotelId,
                    name: "Suite",
                    description: "A luxurious suite with separate living area",
                    pricePerNight: 299.99m,
                    maxGuests: 4,
                    totalRooms: 2
                )
            };

            await unitOfWork.RoomTypes.AddRangeAsync(roomTypes, cancellationToken);
            _logger.LogInformation("Seeded {Count} room types", roomTypes.Count);
        }
    }
}