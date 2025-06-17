using HotelBookingSystem.InventoryService.Application.Common.Models;
using HotelBookingSystem.InventoryService.Application.DTOs;
using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Domain.Repositories;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.InventoryService.Application.Queries.GetRoomHold;

public class GetRoomHoldQueryHandler : IRequestHandler<GetRoomHoldQuery, Result<RoomHoldDto>>
{
    private readonly IRoomHoldRepository _roomHoldRepository;
    private readonly ILogger<GetRoomHoldQueryHandler> _logger;

    public GetRoomHoldQueryHandler(
        IRoomHoldRepository roomHoldRepository,
        ILogger<GetRoomHoldQueryHandler> logger)
    {
        _roomHoldRepository = roomHoldRepository;
        _logger = logger;
    }

    public async Task<Result<RoomHoldDto>> Handle(GetRoomHoldQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting room hold details for hold {HoldId}", request.HoldId);

            var holdId = HoldId.From(request.HoldId);
            var roomHold = await _roomHoldRepository.GetByIdAsync(holdId, cancellationToken);

            if (roomHold == null)
            {
                _logger.LogWarning("Room hold {HoldId} not found", request.HoldId);
                return Result.Failure<RoomHoldDto>("Room hold not found");
            }

            // Verify booking ownership if BookingId is provided
            if (request.BookingId.HasValue && roomHold.BookingId != request.BookingId.Value)
            {
                _logger.LogWarning("Room hold {HoldId} does not belong to booking {BookingId}", 
                    request.HoldId, request.BookingId);
                return Result.Failure<RoomHoldDto>("Room hold not found");
            }

            var roomHoldDto = new RoomHoldDto
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
                CreatedAt = roomHold.CreatedAt,
                ExpiresAt = roomHold.ExpiresAt,
                ConfirmedAt = roomHold.ConfirmedAt,
                ReleasedAt = roomHold.ReleasedAt,
                ReleaseReason = roomHold.ReleaseReason,
                RemainingTime = roomHold.GetRemainingTime(),
                IsExpired = roomHold.IsExpired,
                IsActive = roomHold.IsActive
            };

            _logger.LogInformation("Successfully retrieved room hold {HoldId} with status {Status}", 
                request.HoldId, roomHold.Status);

            return Result.Success(roomHoldDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting room hold {HoldId}", request.HoldId);
            return Result.Failure<RoomHoldDto>($"Failed to get room hold: {ex.Message}");
        }
    }
} 