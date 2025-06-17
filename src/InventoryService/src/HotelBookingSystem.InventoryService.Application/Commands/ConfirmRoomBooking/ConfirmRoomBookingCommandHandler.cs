using HotelBookingSystem.InventoryService.Application.Common.Models;
using HotelBookingSystem.InventoryService.Domain.Repositories;
using HotelBookingSystem.InventoryService.Domain.Services;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.InventoryService.Application.Commands.ConfirmRoomBooking;

public class ConfirmRoomBookingCommandHandler : IRequestHandler<ConfirmRoomBookingCommand, Result>
{
    private readonly IInventoryDomainService _inventoryDomainService;
    private readonly IRoomHoldRepository _roomHoldRepository;
    private readonly ILogger<ConfirmRoomBookingCommandHandler> _logger;

    public ConfirmRoomBookingCommandHandler(
        IInventoryDomainService inventoryDomainService,
        IRoomHoldRepository roomHoldRepository,
        ILogger<ConfirmRoomBookingCommandHandler> logger)
    {
        _inventoryDomainService = inventoryDomainService;
        _roomHoldRepository = roomHoldRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(ConfirmRoomBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing confirm room booking request for booking {BookingId} and hold {HoldId}", 
                request.BookingId, request.HoldId);

            var holdId = HoldId.From(request.HoldId);

            // Verify the hold exists and belongs to the booking
            var roomHold = await _roomHoldRepository.GetByIdAsync(holdId, cancellationToken);
            if (roomHold == null)
            {
                _logger.LogWarning("Room hold {HoldId} not found", request.HoldId);
                return Result.Failure("Room hold not found");
            }

            if (roomHold.BookingId != request.BookingId)
            {
                _logger.LogWarning("Room hold {HoldId} does not belong to booking {BookingId}", 
                    request.HoldId, request.BookingId);
                return Result.Failure("Room hold does not belong to the specified booking");
            }

            if (!roomHold.CanBeConfirmed)
            {
                _logger.LogWarning("Room hold {HoldId} cannot be confirmed. Status: {Status}, IsExpired: {IsExpired}", 
                    request.HoldId, roomHold.Status, roomHold.IsExpired);
                return Result.Failure("Room hold cannot be confirmed. It may have expired or already been processed.");
            }

            // Confirm the booking
            await _inventoryDomainService.ConfirmBookingAsync(holdId, cancellationToken);

            _logger.LogInformation("Successfully confirmed room booking for booking {BookingId} and hold {HoldId}", 
                request.BookingId, request.HoldId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming room booking {BookingId} for hold {HoldId}", 
                request.BookingId, request.HoldId);
            return Result.Failure($"Failed to confirm room booking: {ex.Message}");
        }
    }
} 