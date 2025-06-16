using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;

namespace HotelBookingSystem.InventoryService.Domain.Repositories;

public interface IRoomInventoryRepository
{
    Task<RoomInventory?> GetByIdAsync(InventoryId id, CancellationToken cancellationToken = default);
    
    Task<RoomInventory?> GetByRoomTypeAndDateAsync(
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateTime date,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<RoomInventory>> GetByRoomTypeAndDateRangeAsync(
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateRange dateRange,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<RoomInventory>> GetByHotelAndDateRangeAsync(
        HotelId hotelId,
        DateRange dateRange,
        CancellationToken cancellationToken = default);
    
    Task<bool> CheckAvailabilityAsync(
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateRange dateRange,
        int requiredRooms,
        CancellationToken cancellationToken = default);
    
    void Add(RoomInventory roomInventory);
    
    void Update(RoomInventory roomInventory);
    
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
} 