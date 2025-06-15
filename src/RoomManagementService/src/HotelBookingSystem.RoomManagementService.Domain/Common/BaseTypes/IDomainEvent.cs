namespace HotelBookingSystem.RoomManagementService.Domain.Common.BaseTypes
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTime OccurredOn { get; }
    }
}
