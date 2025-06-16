using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;

namespace HotelBookingSystem.InventoryService.Domain.Events;

public record RoomReleasedDomainEvent(
    HoldId HoldId,
    Guid BookingId,
    int ReleasedRoomCount,
    string Reason
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}; 