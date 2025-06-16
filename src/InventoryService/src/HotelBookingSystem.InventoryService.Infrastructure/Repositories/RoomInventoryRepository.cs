using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Domain.Repositories;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;
using HotelBookingSystem.InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.InventoryService.Infrastructure.Repositories;

public class RoomInventoryRepository : IRoomInventoryRepository
{
    private readonly IInventoryDbContext _context;

    public RoomInventoryRepository(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<RoomInventory?> GetByIdAsync(InventoryId id, CancellationToken cancellationToken = default)
    {
        return await _context.RoomInventories
            .FirstOrDefaultAsync(ri => ri.Id == id, cancellationToken);
    }

    public async Task<RoomInventory?> GetByRoomTypeAndDateAsync(
        HotelId hotelId, 
        RoomTypeId roomTypeId, 
        DateTime date, 
        CancellationToken cancellationToken = default)
    {
        return await _context.RoomInventories
            .FirstOrDefaultAsync(ri => ri.HotelId == hotelId && 
                                      ri.RoomTypeId == roomTypeId && 
                                      ri.Date.Date == date.Date, 
                                cancellationToken);
    }

    public async Task<IEnumerable<RoomInventory>> GetByRoomTypeAndDateRangeAsync(
        HotelId hotelId, 
        RoomTypeId roomTypeId, 
        DateRange dateRange, 
        CancellationToken cancellationToken = default)
    {
        return await _context.RoomInventories
            .Where(ri => ri.HotelId == hotelId && 
                        ri.RoomTypeId == roomTypeId && 
                        ri.Date >= dateRange.CheckIn.Date && 
                        ri.Date < dateRange.CheckOut.Date)
            .OrderBy(ri => ri.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RoomInventory>> GetByHotelAndDateRangeAsync(
        HotelId hotelId, 
        DateRange dateRange, 
        CancellationToken cancellationToken = default)
    {
        return await _context.RoomInventories
            .Where(ri => ri.HotelId == hotelId && 
                        ri.Date >= dateRange.CheckIn.Date && 
                        ri.Date < dateRange.CheckOut.Date)
            .OrderBy(ri => ri.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CheckAvailabilityAsync(
        HotelId hotelId, 
        RoomTypeId roomTypeId, 
        DateRange dateRange, 
        int requiredRooms, 
        CancellationToken cancellationToken = default)
    {
        var inventories = await GetByRoomTypeAndDateRangeAsync(
            hotelId, roomTypeId, dateRange, cancellationToken);

        // Check if all required dates have sufficient availability
        var requiredDates = GetDateRange(dateRange.CheckIn, dateRange.CheckOut);
        
        foreach (var date in requiredDates)
        {
            var inventory = inventories.FirstOrDefault(i => i.Date.Date == date.Date);
            if (inventory == null || inventory.AvailableRooms < requiredRooms)
            {
                return false;
            }
        }

        return true;
    }

    public void Add(RoomInventory roomInventory)
    {
        _context.RoomInventories.Add(roomInventory);
    }

    public void Update(RoomInventory roomInventory)
    {
        _context.RoomInventories.Update(roomInventory);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static List<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
    {
        var dates = new List<DateTime>();
        var currentDate = startDate.Date;
        
        while (currentDate < endDate.Date)
        {
            dates.Add(currentDate);
            currentDate = currentDate.AddDays(1);
        }
        
        return dates;
    }
} 