using HotelBookingSystem.Contracts.Commands;
using HotelBookingSystem.Contracts.Events;
using HotelBookingSystem.InventoryService.Application.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.InventoryService.Infrastructure.Consumers
{
    public class HoldRoomConsumer : IConsumer<HoldRoom>
    {
        private readonly ILogger<HoldRoomConsumer> _logger;
        private readonly IRoomInventoryService _inventoryService;

        public HoldRoomConsumer(
            ILogger<HoldRoomConsumer> logger, 
            IRoomInventoryService inventoryService)
        {
            _logger = logger;
            _inventoryService = inventoryService;
        }

        public async Task Consume(ConsumeContext<HoldRoom> context)
        {
            var command = context.Message;

            _logger.LogInformation("Processing room hold request for Booking: {BookingId}, Hotel: {HotelId}, RoomType: {RoomTypeId}", 
                command.BookingId, command.HotelId, command.RoomTypeId);

            try
            {
                var result = await _inventoryService.HoldRoomAsync(
                    command.BookingId,
                    command.RoomTypeId,
                    command.CheckInDate,
                    command.CheckOutDate,
                    command.HoldDuration);

                if (result.Success)
                {
                    _logger.LogInformation("Room hold request for Booking: {BookingId} successful. Hold reference: {HoldReference}", 
                        command.BookingId, result.HoldReference);

                    // Publish success event
                    await context.Publish<RoomHeld>(new
                    {
                        BookingId = command.BookingId,
                        RoomHoldReference = result.HoldReference,
                        HeldUntil = DateTime.UtcNow.Add(command.HoldDuration),
                        RoomNumber = $"Room-{new Random().Next(101, 199)}" // Mock room number
                    });
                }
                else
                {
                    _logger.LogWarning("Room hold request for Booking: {BookingId} failed: {ErrorMessage}", 
                        command.BookingId, result.ErrorMessage);
                    
                    // Publish failure event
                    await context.Publish<RoomHoldFailed>(new
                    {
                        BookingId = command.BookingId,
                        Reason = result.ErrorMessage ?? "Room hold failed",
                        FailedAt = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing room hold request for Booking: {BookingId}, Hotel: {HotelId}, RoomType: {RoomTypeId}", 
                    command.BookingId, command.HotelId, command.RoomTypeId);
                
                // Publish failure event on exception
                await context.Publish<RoomHoldFailed>(new
                {
                    BookingId = command.BookingId,
                    Reason = $"System error: {ex.Message}",
                    FailedAt = DateTime.UtcNow
                });
            }
        }
    }
} 