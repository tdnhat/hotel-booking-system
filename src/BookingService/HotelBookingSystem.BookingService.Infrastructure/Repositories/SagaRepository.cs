using HotelBookingSystem.BookingService.Application.Interfaces;
using HotelBookingSystem.BookingService.Domain.Entities;
using HotelBookingSystem.BookingService.Domain.Enums;
using HotelBookingSystem.BookingService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.BookingService.Infrastructure.Repositories
{
    public class SagaRepository : ISagaRepository
    {
        private readonly IBookingDbContext _context;

        public SagaRepository(IBookingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<BookingState>> GetActiveBookingsAsync(CancellationToken cancellationToken = default)
        {
            var activeStates = new[]
            {
                nameof(BookingStatus.Submitted),
                nameof(BookingStatus.RoomHoldRequested),
                nameof(BookingStatus.RoomHeld),
                nameof(BookingStatus.PaymentProcessing),
                nameof(BookingStatus.RoomReleaseRequested),
            };
            return await _context.BookingStates
                .Where(bs => activeStates.Contains(bs.CurrentState))
                .ToListAsync(cancellationToken);
        }

        public async Task<BookingState?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.BookingStates
                .FirstOrDefaultAsync(bs => bs.BookingId == bookingId, cancellationToken);
        }

        public async Task<BookingState?> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default)
        {
            return await _context.BookingStates
                .FirstOrDefaultAsync(bs => bs.CorrelationId == correlationId, cancellationToken);
        }

        public async Task<IEnumerable<BookingState>> GetByGuestEmailAsync(string guestEmail, CancellationToken cancellationToken = default)
        {
            return await _context.BookingStates
                .Where(bs => bs.GuestEmail == guestEmail)
                .OrderByDescending(bs => bs.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}