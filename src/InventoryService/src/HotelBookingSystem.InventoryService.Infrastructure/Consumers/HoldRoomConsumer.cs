using HotelBookingSystem.Contracts.Commands;
using HotelBookingSystem.Contracts.Events;
using HotelBookingSystem.InventoryService.Application.Commands.HoldRoom;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.InventoryService.Infrastructure.Consumers
{
    public class HoldRoomConsumer : IConsumer<HoldRoom>
    {
        private readonly ILogger<HoldRoomConsumer> _logger;
        private readonly ISender _sender;

        public HoldRoomConsumer(
            ILogger<HoldRoomConsumer> logger,
            ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        public async Task Consume(ConsumeContext<HoldRoom> context)
        {
            var command = context.Message;

            _logger.LogInformation("Processing room hold request for Booking: {BookingId}, Hotel: {HotelId}, RoomType: {RoomTypeId}",
                command.BookingId, command.HotelId, command.RoomTypeId);

            try
            {
                var holdRoomCommand = new HoldRoomCommand
                {
                    BookingId = command.BookingId,
                    HotelId = command.HotelId,
                    RoomTypeId = command.RoomTypeId,
                    CheckIn = command.CheckInDate,
                    CheckOut = command.CheckOutDate,
                    RoomCount = 1, // Default to 1 room since contract doesn't specify room count
                    HoldDuration = command.HoldDuration
                };

                var result = await _sender.Send(holdRoomCommand, context.CancellationToken);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Room hold request for Booking: {BookingId} successful. Hold reference: {HoldReference}",
                        command.BookingId, result.Value.HoldReference);

                    // Publish success event
                    await context.Publish<RoomHeld>(new
                    {
                        BookingId = command.BookingId,
                        RoomHoldReference = result.Value.HoldReference,
                        HeldUntil = result.Value.ExpiresAt,
                        RoomNumber = $"Room-{new Random().Next(101, 199)}" // Mock room number for now
                    }, context.CancellationToken);
                }
                else
                {
                    _logger.LogWarning("Room hold request for Booking: {BookingId} failed: {ErrorMessage}",
                        command.BookingId, result.Error);

                    // Publish failure event
                    await context.Publish<RoomHoldFailed>(new
                    {
                        BookingId = command.BookingId,
                        Reason = result.Error,
                        FailedAt = DateTime.UtcNow
                    }, context.CancellationToken);
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
                }, context.CancellationToken);
            }
        }
    }
}