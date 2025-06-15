namespace HotelBookingSystem.RoomManagementService.Application.Common.Abstractions
{
    public interface IDateTime
    {
         DateTime Now { get; }
         DateTime UtcNow { get; }
         DateOnly Today { get; }
    }
}