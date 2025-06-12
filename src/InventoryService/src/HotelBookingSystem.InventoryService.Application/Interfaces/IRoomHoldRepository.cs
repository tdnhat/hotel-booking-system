using HotelBookingSystem.InventoryService.Domain.Entities;

namespace HotelBookingSystem.InventoryService.Application.Interfaces
{
    public interface IRoomHoldRepository
    {
        Task<RoomHold?> GetActiveHoldByBookingIdAsync(Guid bookingId);
        Task<RoomHold?> GetByHoldReferenceAsync(string holdReference);
        Task CreateAsync(RoomHold roomHold);
        Task UpdateAsync(RoomHold roomHold);
    }
} 