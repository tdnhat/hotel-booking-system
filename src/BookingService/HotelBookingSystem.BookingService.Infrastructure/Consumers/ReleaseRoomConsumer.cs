using HotelBookingSystem.BookingService.Domain.Messages.Commands;
using HotelBookingSystem.BookingService.Domain.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.BookingService.Infrastructure.Consumers
{
    public class ReleaseRoomConsumer : IConsumer<ReleaseRoom>
    {
        private readonly ILogger<ReleaseRoomConsumer> _logger;
        public ReleaseRoomConsumer(ILogger<ReleaseRoomConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReleaseRoom> context)
        {
            var command = context.Message;

            _logger.LogInformation("Releasing room for Booking: {BookingId}, Room: {RoomNumber}", 
                command.BookingId, command.RoomHoldReference);

            // TODO: Implement room release logic
            try
            {
                await Task.Delay(500, context.CancellationToken);

                var random = new Random();
                var success = random.NextDouble() < 0.99; // 99% chance of success

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
                    _logger.LogError("Room release request for Booking: {BookingId} failed", command.BookingId);

                    // Publish failure event
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

                // Publish failure event
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