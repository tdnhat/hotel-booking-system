using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;

namespace HotelBookingSystem.RoomManagementService.Domain.Events
{
    public record RoomTypeAddedDomainEvent(
    HotelId HotelId,
    RoomTypeId RoomTypeId,
    string Name,
    int Capacity,
    decimal BasePrice) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
