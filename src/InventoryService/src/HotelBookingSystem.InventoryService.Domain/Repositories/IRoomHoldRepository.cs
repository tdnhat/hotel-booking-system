using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;

namespace HotelBookingSystem.InventoryService.Domain.Repositories;

public interface IRoomHoldRepository
{
    Task<RoomHold?> GetByIdAsync(HoldId id, CancellationToken cancellationToken = default);
    
    Task<RoomHold?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    
    Task<RoomHold?> GetByHoldReferenceAsync(string holdReference, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<RoomHold>> GetActiveHoldsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<RoomHold>> GetExpiredHoldsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<RoomHold>> GetHoldsByStatusAsync(
        HoldStatus status, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<RoomHold>> GetHoldsByRoomTypeAsync(
        HotelId hotelId,
        RoomTypeId roomTypeId,
        CancellationToken cancellationToken = default);
    
    void Add(RoomHold roomHold);
    
    void Update(RoomHold roomHold);
    
    void Remove(RoomHold roomHold);
    
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
} 