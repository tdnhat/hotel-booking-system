using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Domain.Exceptions;
using HotelBookingSystem.InventoryService.Domain.Repositories;
using HotelBookingSystem.InventoryService.Domain.Services;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.InventoryService.Infrastructure.Services;

public class InventoryDomainService : IInventoryDomainService
{
    private readonly IRoomInventoryRepository _roomInventoryRepository;
    private readonly IRoomHoldRepository _roomHoldRepository;
    private readonly ILogger<InventoryDomainService> _logger;

    public InventoryDomainService(
        IRoomInventoryRepository roomInventoryRepository,
        IRoomHoldRepository roomHoldRepository,
        ILogger<InventoryDomainService> logger)
    {
        _roomInventoryRepository = roomInventoryRepository;
        _roomHoldRepository = roomHoldRepository;
        _logger = logger;
    }

    public async Task<RoomHold> HoldRoomsAsync(
        Guid bookingId,
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateRange dateRange,
        int roomCount,
        TimeSpan holdDuration,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating room hold for booking {BookingId}, hotel {HotelId}, room type {RoomTypeId}, date range {DateRange}, room count {RoomCount}",
            bookingId, hotelId.Value, roomTypeId.Value, dateRange, roomCount);

        // Check availability
        var canHold = await CanHoldRoomsAsync(hotelId, roomTypeId, dateRange, roomCount, cancellationToken);
        if (!canHold)
        {
            throw new InsufficientInventoryException(
                $"Insufficient inventory for hotel {hotelId.Value}, room type {roomTypeId.Value}, dates {dateRange}, room count {roomCount}");
        }

        // Calculate total amount
        var totalAmount = await CalculateTotalAmountAsync(hotelId, roomTypeId, dateRange, roomCount, cancellationToken);

        // Create the hold
        var holdId = HoldId.From(Guid.NewGuid());
        var holdReference = GenerateHoldReference();

        var roomHold = new RoomHold(
            holdId,
            bookingId,
            hotelId,
            roomTypeId,
            dateRange,
            roomCount,
            totalAmount,
            holdDuration,
            holdReference);

        // Update inventory to reflect the hold
        await UpdateInventoryForHoldAsync(hotelId, roomTypeId, dateRange, roomCount, cancellationToken);

        _roomHoldRepository.Add(roomHold);
        await _roomHoldRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Room hold created successfully: {HoldId} with reference {HoldReference} for booking {BookingId}",
            holdId.Value, holdReference, bookingId);

        return roomHold;
    }

    public async Task<bool> CanHoldRoomsAsync(
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateRange dateRange,
        int roomCount,
        CancellationToken cancellationToken = default)
    {
        return await _roomInventoryRepository.CheckAvailabilityAsync(
            hotelId, roomTypeId, dateRange, roomCount, cancellationToken);
    }

    public async Task ReleaseHoldAsync(
        HoldId holdId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Releasing room hold {HoldId} with reason: {Reason}", holdId.Value, reason);

        var roomHold = await _roomHoldRepository.GetByIdAsync(holdId, cancellationToken);
        if (roomHold == null)
        {
            throw new HoldNotFoundException($"Hold {holdId.Value} not found");
        }

        if (!roomHold.CanBeReleased)
        {
            throw new InvalidHoldOperationException($"Cannot release hold {holdId.Value} with status {roomHold.Status}");
        }

        // Release the hold
        roomHold.Release(reason);

        // Update inventory to reflect the release
        await UpdateInventoryForReleaseAsync(
            roomHold.HotelId, 
            roomHold.RoomTypeId, 
            roomHold.DateRange, 
            roomHold.RoomCount, 
            cancellationToken);

        _roomHoldRepository.Update(roomHold);
        await _roomHoldRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Room hold {HoldId} released successfully", holdId.Value);
    }

    public async Task ConfirmBookingAsync(
        HoldId holdId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Confirming room hold {HoldId}", holdId.Value);

        var roomHold = await _roomHoldRepository.GetByIdAsync(holdId, cancellationToken);
        if (roomHold == null)
        {
            throw new HoldNotFoundException($"Hold {holdId.Value} not found");
        }

        if (!roomHold.CanBeConfirmed)
        {
            throw new InvalidHoldOperationException($"Cannot confirm hold {holdId.Value} with status {roomHold.Status}");
        }

        // Confirm the hold
        roomHold.Confirm();

        // Update inventory to reflect the confirmation (held -> booked)
        await UpdateInventoryForConfirmationAsync(
            roomHold.HotelId, 
            roomHold.RoomTypeId, 
            roomHold.DateRange, 
            roomHold.RoomCount, 
            cancellationToken);

        _roomHoldRepository.Update(roomHold);
        await _roomHoldRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Room hold {HoldId} confirmed successfully", holdId.Value);
    }

    public async Task<Money> CalculateTotalAmountAsync(
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateRange dateRange,
        int roomCount,
        CancellationToken cancellationToken = default)
    {
        var inventories = await _roomInventoryRepository.GetByRoomTypeAndDateRangeAsync(
            hotelId, roomTypeId, dateRange, cancellationToken);

        var total = Money.Zero(Currency.USD);
        var currentDate = dateRange.CheckIn.Date;

        while (currentDate < dateRange.CheckOut.Date)
        {
            var inventory = inventories.FirstOrDefault(i => i.Date.Date == currentDate);
            if (inventory != null)
            {
                total = total.Add(inventory.CurrentPrice.Multiply(roomCount));
            }
            currentDate = currentDate.AddDays(1);
        }

        return total;
    }

    public async Task ProcessExpiredHoldsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing expired holds");

        var expiredHolds = await _roomHoldRepository.GetExpiredHoldsAsync(cancellationToken);
        
        foreach (var hold in expiredHolds)
        {
            _logger.LogInformation("Processing expired hold {HoldId}", hold.Id.Value);

            hold.MarkAsExpired();

            // Update inventory to reflect the expiration
            await UpdateInventoryForReleaseAsync(
                hold.HotelId, 
                hold.RoomTypeId, 
                hold.DateRange, 
                hold.RoomCount, 
                cancellationToken);

            _roomHoldRepository.Update(hold);
        }

        if (expiredHolds.Any())
        {
            await _roomHoldRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Processed {Count} expired holds", expiredHolds.Count());
        }
    }

    private async Task UpdateInventoryForHoldAsync(
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateRange dateRange,
        int roomCount,
        CancellationToken cancellationToken)
    {
        var inventories = await _roomInventoryRepository.GetByRoomTypeAndDateRangeAsync(
            hotelId, roomTypeId, dateRange, cancellationToken);

        var currentDate = dateRange.CheckIn.Date;
        while (currentDate < dateRange.CheckOut.Date)
        {
            var inventory = inventories.FirstOrDefault(i => i.Date.Date == currentDate);
            if (inventory != null)
            {
                inventory.HoldRooms(roomCount);
                _roomInventoryRepository.Update(inventory);
            }
            currentDate = currentDate.AddDays(1);
        }

        await _roomInventoryRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateInventoryForReleaseAsync(
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateRange dateRange,
        int roomCount,
        CancellationToken cancellationToken)
    {
        var inventories = await _roomInventoryRepository.GetByRoomTypeAndDateRangeAsync(
            hotelId, roomTypeId, dateRange, cancellationToken);

        var currentDate = dateRange.CheckIn.Date;
        while (currentDate < dateRange.CheckOut.Date)
        {
            var inventory = inventories.FirstOrDefault(i => i.Date.Date == currentDate);
            if (inventory != null)
            {
                inventory.ReleaseHold(roomCount);
                _roomInventoryRepository.Update(inventory);
            }
            currentDate = currentDate.AddDays(1);
        }

        await _roomInventoryRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateInventoryForConfirmationAsync(
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateRange dateRange,
        int roomCount,
        CancellationToken cancellationToken)
    {
        var inventories = await _roomInventoryRepository.GetByRoomTypeAndDateRangeAsync(
            hotelId, roomTypeId, dateRange, cancellationToken);

        var currentDate = dateRange.CheckIn.Date;
        while (currentDate < dateRange.CheckOut.Date)
        {
            var inventory = inventories.FirstOrDefault(i => i.Date.Date == currentDate);
            if (inventory != null)
            {
                inventory.ConfirmBooking(roomCount);
                _roomInventoryRepository.Update(inventory);
            }
            currentDate = currentDate.AddDays(1);
        }

        await _roomInventoryRepository.SaveChangesAsync(cancellationToken);
    }

    private static string GenerateHoldReference()
    {
        return $"HOLD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..20];
    }
} 