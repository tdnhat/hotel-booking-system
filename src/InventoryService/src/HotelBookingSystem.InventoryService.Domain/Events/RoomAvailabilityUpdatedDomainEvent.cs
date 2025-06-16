using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;

namespace HotelBookingSystem.InventoryService.Domain.Events;

public record RoomAvailabilityUpdatedDomainEvent(
    HotelId HotelId,
    RoomTypeId RoomTypeId,
    DateTime Date,
    int AvailableRooms,
    int TotalRooms,
    Money CurrentPrice
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}; 