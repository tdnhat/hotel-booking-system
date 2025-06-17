using HotelBookingSystem.InventoryService.Application.Common.Models;
using HotelBookingSystem.InventoryService.Application.DTOs;
using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Domain.Repositories;
using HotelBookingSystem.InventoryService.Domain.Services;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.InventoryService.Application.Commands.HoldRoom;

public class HoldRoomCommandHandler : IRequestHandler<HoldRoomCommand, Result<RoomHoldDto>>
{
    private readonly IInventoryDomainService _inventoryDomainService;
    private readonly IRoomHoldRepository _roomHoldRepository;
    private readonly ILogger<HoldRoomCommandHandler> _logger;

    public HoldRoomCommandHandler(
        IInventoryDomainService inventoryDomainService,
        IRoomHoldRepository roomHoldRepository,
        ILogger<HoldRoomCommandHandler> logger)
    {
        _inventoryDomainService = inventoryDomainService;
        _roomHoldRepository = roomHoldRepository;
        _logger = logger;
    }

    public async Task<Result<RoomHoldDto>> Handle(HoldRoomCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing hold room request for booking {BookingId}", request.BookingId);

            var hotelId = HotelId.From(request.HotelId);
            var roomTypeId = RoomTypeId.From(request.RoomTypeId);
            var dateRange = new DateRange(request.CheckIn, request.CheckOut);

            // Check if rooms can be held
            var canHold = await _inventoryDomainService.CanHoldRoomsAsync(
                hotelId, roomTypeId, dateRange, request.RoomCount, cancellationToken);

            if (!canHold)
            {
                _logger.LogWarning("Insufficient inventory for booking {BookingId}", request.BookingId);
                return Result.Failure<RoomHoldDto>("Insufficient inventory available for the requested dates and room count");
            }

            // Create the hold
            var roomHold = await _inventoryDomainService.HoldRoomsAsync(
                request.BookingId,
                hotelId,
                roomTypeId,
                dateRange,
                request.RoomCount,
                request.HoldDuration,
                cancellationToken);

            // Convert to DTO
            var roomHoldDto = MapToDto(roomHold);

            _logger.LogInformation("Successfully created room hold {HoldId} for booking {BookingId}", 
                roomHold.Id, request.BookingId);

            return Result.Success(roomHoldDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating room hold for booking {BookingId}", request.BookingId);
            return Result.Failure<RoomHoldDto>($"Failed to create room hold: {ex.Message}");
        }
    }    private static RoomHoldDto MapToDto(RoomHold roomHold)
    {
        return new RoomHoldDto
        {
            Id = roomHold.Id.Value,
            BookingId = roomHold.BookingId,
            HotelId = roomHold.HotelId.Value,
            RoomTypeId = roomHold.RoomTypeId.Value,
            CheckIn = roomHold.DateRange.CheckIn,
            CheckOut = roomHold.DateRange.CheckOut,
            RoomCount = roomHold.RoomCount,
            TotalAmount = roomHold.TotalAmount.Amount,
            Currency = roomHold.TotalAmount.Currency.ToString(),
            Status = roomHold.Status.ToString(),
            HoldReference = roomHold.HoldReference, // This was missing!
            CreatedAt = roomHold.CreatedAt,
            ExpiresAt = roomHold.ExpiresAt,
            ConfirmedAt = roomHold.ConfirmedAt,
            ReleasedAt = roomHold.ReleasedAt,
            ReleaseReason = roomHold.ReleaseReason,
            RemainingTime = roomHold.GetRemainingTime(),
            IsExpired = roomHold.IsExpired,
            IsActive = roomHold.IsActive
        };
    }
} 