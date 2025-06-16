using HotelBookingSystem.Contracts.Commands;
using HotelBookingSystem.Contracts.Events;
using HotelBookingSystem.InventoryService.Application.Commands.ReleaseRoomHold;
using HotelBookingSystem.InventoryService.Domain.Repositories;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.InventoryService.Infrastructure.Consumers
{
    public class ReleaseRoomConsumer : IConsumer<ReleaseRoom>
    {
        private readonly ILogger<ReleaseRoomConsumer> _logger;
        private readonly ISender _sender;
        private readonly IRoomHoldRepository _roomHoldRepository;

        public ReleaseRoomConsumer(
            ILogger<ReleaseRoomConsumer> logger,
            ISender sender,
            IRoomHoldRepository roomHoldRepository)
        {
            _logger = logger;
            _sender = sender;
            _roomHoldRepository = roomHoldRepository;
        }

        public async Task Consume(ConsumeContext<ReleaseRoom> context)
        {
            var command = context.Message;

            _logger.LogInformation("Releasing room for Booking: {BookingId}, Room: {RoomHoldReference}",
                command.BookingId, command.RoomHoldReference);

            try
            {
                // Find the hold by hold reference and convert to hold ID for the command
                var roomHold = await _roomHoldRepository.GetByHoldReferenceAsync(command.RoomHoldReference, context.CancellationToken);
                if (roomHold == null)
                {
                    _logger.LogWarning("Room hold with reference {HoldReference} not found for booking {BookingId}",
                        command.RoomHoldReference, command.BookingId);

                    // Still publish event for saga completion
                    await context.Publish<RoomReleased>(new
                    {
                        BookingId = command.BookingId,
                        RoomHoldReference = command.RoomHoldReference,
                        ReleasedAt = DateTime.UtcNow,
                        Reason = "Hold not found - already released or expired"
                    }, context.CancellationToken);
                    return;
                }

                var releaseCommand = new ReleaseRoomHoldCommand
                {
                    BookingId = command.BookingId,
                    HoldId = roomHold.Id.Value,
                    Reason = "Saga compensation - payment failed or booking cancelled"
                };

                var result = await _sender.Send(releaseCommand, context.CancellationToken);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Room release request for Booking: {BookingId} successful", command.BookingId);

                    // Publish success event
                    await context.Publish<RoomReleased>(new
                    {
                        BookingId = command.BookingId,
                        RoomHoldReference = command.RoomHoldReference,
                        ReleasedAt = DateTime.UtcNow,
                        Reason = "Compensation - payment failed"
                    }, context.CancellationToken);
                }
                else
                {
                    _logger.LogWarning("Room release request for Booking: {BookingId} failed: {Error}",
                        command.BookingId, result.Error);

                    // Even if release failed, we publish the event for saga completion
                    await context.Publish<RoomReleased>(new
                    {
                        BookingId = command.BookingId,
                        RoomHoldReference = command.RoomHoldReference,
                        ReleasedAt = DateTime.UtcNow,
                        Reason = $"Release failed: {result.Error} - marked as complete for saga completion"
                    }, context.CancellationToken);
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
                }, context.CancellationToken);
            }
        }
    }
}