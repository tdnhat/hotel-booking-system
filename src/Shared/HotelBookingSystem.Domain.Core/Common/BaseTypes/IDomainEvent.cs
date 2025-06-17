namespace HotelBookingSystem.Domain.Core.Common.BaseTypes
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTime OccurredOn { get; }
    }
}
