using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;

namespace HotelBookingSystem.RoomManagementService.Domain.Events
{
    public record HotelCreatedDomainEvent(
        HotelId HotelId,
        string Name,
        string Address) : IDomainEvent
        {
        public Guid EventId { get; } = Guid.NewGuid();

        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
