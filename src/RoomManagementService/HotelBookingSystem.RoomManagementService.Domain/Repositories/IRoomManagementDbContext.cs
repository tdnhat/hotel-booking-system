using HotelBookingSystem.RoomManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.RoomManagementService.Domain.Repositories
{
    public interface IRoomManagementDbContext
    {
        DbSet<RoomType> RoomTypes { get; }
        DbSet<Room> Rooms { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
} 