using HotelBookingSystem.BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.BookingService.Domain.Repositories
{
    public interface IBookingDbContext
    {
        DbSet<BookingState> BookingStates { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}