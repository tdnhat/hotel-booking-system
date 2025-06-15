using HotelBookingSystem.RoomManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Data
{
    public interface IRoomManagementDbContext
    {
        DbSet<Hotel> Hotels { get; set; }
        DbSet<RoomType> RoomTypes { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
} 