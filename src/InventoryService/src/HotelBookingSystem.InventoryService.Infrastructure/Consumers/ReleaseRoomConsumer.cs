using HotelBookingSystem.Contracts.Commands;
using HotelBookingSystem.Contracts.Events;
using HotelBookingSystem.InventoryService.Application.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.InventoryService.Infrastructure.Consumers
{
    public class ReleaseRoomConsumer : IConsumer<ReleaseRoom>
    {
        private readonly ILogger<ReleaseRoomConsumer> _logger;
        private readonly IRoomInventoryService _inventoryService;

        public ReleaseRoomConsumer(
            ILogger<ReleaseRoomConsumer> logger, 
            IRoomInventoryService inventoryService)
        {
            _logger = logger;
            _inventoryService = inventoryService;
        }

        public async Task Consume(ConsumeContext<ReleaseRoom> context)
        {
            var command = context.Message;

            _logger.LogInformation("Releasing room for Booking: {BookingId}, Room: {RoomHoldReference}", 
                command.BookingId, command.RoomHoldReference);

            try
            {
                var success = await _inventoryService.ReleaseRoomAsync(command.BookingId, command.RoomHoldReference);

                if (success)
                {
                    _logger.LogInformation("Room release request for Booking: {BookingId} successful", command.BookingId);

                    // Publish success event
                    await context.Publish<RoomReleased>(new
                    {
                        BookingId = command.BookingId,
                        RoomHoldReference = command.RoomHoldReference,
                        ReleasedAt = DateTime.UtcNow,
                        Reason = "Compensation - payment failed"
                    });
                }
                else
                {
                    _logger.LogWarning("Room release request for Booking: {BookingId} failed", command.BookingId);

                    // Even if release failed, we publish the event for saga completion
                    await context.Publish<RoomReleased>(new
                    {
                        BookingId = command.BookingId,
                        RoomHoldReference = command.RoomHoldReference,
                        ReleasedAt = DateTime.UtcNow,
                        Reason = "Release failed but marked as complete for saga completion"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing room for Booking: {BookingId}", command.BookingId);

                // Publish event for saga completion even on error
                await context.Publish<RoomReleased>(new
                {
                    BookingId = command.BookingId,
                    RoomHoldReference = command.RoomHoldReference,
                    ReleasedAt = DateTime.UtcNow,
                    Reason = $"System error: {ex.Message}"
                });
            }
        }
    }
} 