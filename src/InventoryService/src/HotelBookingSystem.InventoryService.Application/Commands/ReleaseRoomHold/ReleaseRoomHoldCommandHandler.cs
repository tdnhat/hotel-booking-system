using HotelBookingSystem.InventoryService.Application.Common.Models;
using HotelBookingSystem.InventoryService.Domain.Repositories;
using HotelBookingSystem.InventoryService.Domain.Services;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.InventoryService.Application.Commands.ReleaseRoomHold;

public class ReleaseRoomHoldCommandHandler : IRequestHandler<ReleaseRoomHoldCommand, Result>
{
    private readonly IInventoryDomainService _inventoryDomainService;
    private readonly IRoomHoldRepository _roomHoldRepository;
    private readonly ILogger<ReleaseRoomHoldCommandHandler> _logger;

    public ReleaseRoomHoldCommandHandler(
        IInventoryDomainService inventoryDomainService,
        IRoomHoldRepository roomHoldRepository,
        ILogger<ReleaseRoomHoldCommandHandler> logger)
    {
        _inventoryDomainService = inventoryDomainService;
        _roomHoldRepository = roomHoldRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(ReleaseRoomHoldCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing release room hold request for booking {BookingId} and hold {HoldId}", 
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

            // Release the hold
            await _inventoryDomainService.ReleaseHoldAsync(holdId, request.Reason, cancellationToken);

            _logger.LogInformation("Successfully released room hold {HoldId} for booking {BookingId}", 
                request.HoldId, request.BookingId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing room hold {HoldId} for booking {BookingId}", 
                request.HoldId, request.BookingId);
            return Result.Failure($"Failed to release room hold: {ex.Message}");
        }
    }
} 