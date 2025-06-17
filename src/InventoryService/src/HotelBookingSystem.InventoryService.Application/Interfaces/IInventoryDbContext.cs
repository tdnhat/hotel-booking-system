using HotelBookingSystem.InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.InventoryService.Application.Interfaces
{
    public interface IInventoryDbContext
    {
        DbSet<RoomInventory> RoomInventories { get; }
        DbSet<RoomHold> RoomHolds { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
} 