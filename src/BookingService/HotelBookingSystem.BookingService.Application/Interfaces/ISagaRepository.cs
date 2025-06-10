using HotelBookingSystem.BookingService.Domain.Entities;

namespace HotelBookingSystem.BookingService.Application.Interfaces
{
    public interface ISagaRepository
    {
        Task<BookingState?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
        Task<BookingState?> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default);
        Task<IEnumerable<BookingState>> GetByGuestEmailAsync(string guestEmail, CancellationToken cancellationToken = default);
        Task<IEnumerable<BookingState>> GetActiveBookingsAsync(CancellationToken cancellationToken = default);
    }
}