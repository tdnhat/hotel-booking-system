using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;

namespace HotelBookingSystem.InventoryService.Domain.Events;

public record BookingConfirmedDomainEvent(
    HoldId HoldId,
    Guid BookingId,
    HotelId HotelId,
    RoomTypeId RoomTypeId,
    DateRange DateRange,
    int RoomCount,
    Money TotalAmount
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}; 