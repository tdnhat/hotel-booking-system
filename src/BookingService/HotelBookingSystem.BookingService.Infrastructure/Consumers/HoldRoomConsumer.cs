using HotelBookingSystem.BookingService.Domain.Messages.Commands;
using HotelBookingSystem.BookingService.Domain.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.BookingService.Infrastructure.Consumers
{
    public class HoldRoomConsumer : IConsumer<HoldRoom>
    {
        private readonly ILogger<HoldRoomConsumer> _logger;
        public HoldRoomConsumer(ILogger<HoldRoomConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<HoldRoom> context)
        {
            var command = context.Message;

            _logger.LogInformation("Processing room hold request for Booking: {BookingId}, Hotel: {HotelId}, RoomType: {RoomTypeId}", 
                command.BookingId, command.HotelId, command.RoomTypeId);

            try
            {
                // TODO: Implement room hold logic
                
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