using HotelBookingSystem.RoomManagementService.Domain.Common.BaseTypes;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;

namespace HotelBookingSystem.RoomManagementService.Domain.Events
{
    public record HotelUpdatedDomainEvent(
    HotelId HotelId,
    string Name,
    string Address) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
