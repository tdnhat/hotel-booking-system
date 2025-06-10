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
                await Task.Delay(1000, context.CancellationToken);

                var random = new Random();
                var success = random.NextDouble() < 0.8; // 80% chance of success

                if (success)
                {
                    // Generate a mock room hold reference
                    var holdReference = $"HOLD-{command.BookingId.ToString("N")[..8]}-{DateTime.UtcNow:yyyyMMddHHmmss}";
                    
                    _logger.LogInformation("Room hold request for Booking: {BookingId} successful. Hold reference: {HoldReference}", 
                        command.BookingId, holdReference);

                    // Publish success event
                    await context.Publish<RoomHeld>(new
                    {
                        BookingId = command.BookingId,
                        RoomHoldReference = holdReference,
                        HeldUntil = DateTime.UtcNow.AddMinutes(10),
                        RoomNumber = $"Room-{random.Next(101, 199)}"
                    });
                }
                else
                {
                    _logger.LogError("Room hold request for Booking: {BookingId} failed", command.BookingId);
                    
                    // Publish failure event
                    await context.Publish<RoomHoldFailed>(new
                    {
                        BookingId = command.BookingId,
                        Reason = "No rooms available for the requested dates",
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