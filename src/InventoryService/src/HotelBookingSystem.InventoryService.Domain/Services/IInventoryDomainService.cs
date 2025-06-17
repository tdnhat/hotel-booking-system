using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;

namespace HotelBookingSystem.InventoryService.Domain.Services;

public interface IInventoryDomainService
{
    Task<RoomHold> HoldRoomsAsync(
        Guid bookingId,
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateRange dateRange,
        int roomCount,
        TimeSpan holdDuration,
        CancellationToken cancellationToken = default);

    Task<bool> CanHoldRoomsAsync(
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateRange dateRange,
        int roomCount,
        CancellationToken cancellationToken = default);

    Task ReleaseHoldAsync(
        HoldId holdId,
        string reason,
        CancellationToken cancellationToken = default);

    Task ConfirmBookingAsync(
        HoldId holdId,
        CancellationToken cancellationToken = default);

    Task<Money> CalculateTotalAmountAsync(
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateRange dateRange,
        int roomCount,
        CancellationToken cancellationToken = default);

    Task ProcessExpiredHoldsAsync(CancellationToken cancellationToken = default);
} 