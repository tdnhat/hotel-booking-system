using HotelBookingSystem.InventoryService.Domain.Enums;

namespace HotelBookingSystem.InventoryService.Application.Interfaces
{
    public interface IRoomAvailabilityRepository
    {
        Task<bool> IsRoomAvailableAsync(Guid roomTypeId, List<DateTime> dates);
        Task UpdateRoomStatusAsync(Guid roomTypeId, List<DateTime> dates, AvailabilityStatus status, Guid? bookingId);
    }
} 