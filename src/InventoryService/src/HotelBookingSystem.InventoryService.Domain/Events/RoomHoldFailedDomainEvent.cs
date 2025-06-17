using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;

namespace HotelBookingSystem.InventoryService.Domain.Events;

public record RoomHoldFailedDomainEvent(
    Guid BookingId,
    HotelId HotelId,
    RoomTypeId RoomTypeId,
    DateRange DateRange,
    int RequestedRoomCount,
    int AvailableRoomCount,
    string Reason
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}; 