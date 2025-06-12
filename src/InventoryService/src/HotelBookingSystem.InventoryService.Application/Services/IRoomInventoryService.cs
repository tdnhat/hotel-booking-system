namespace HotelBookingSystem.InventoryService.Application.Services
{
    public interface IRoomInventoryService
    {
        Task<(bool Success, string? HoldReference, string? ErrorMessage)> HoldRoomAsync(
            Guid bookingId, 
            Guid roomTypeId, 
            DateTime checkInDate, 
            DateTime checkOutDate, 
            TimeSpan holdDuration);

        Task<bool> ReleaseRoomAsync(Guid bookingId, string holdReference);

        Task<bool> IsRoomAvailableAsync(Guid roomTypeId, DateTime checkInDate, DateTime checkOutDate);
    }
} 