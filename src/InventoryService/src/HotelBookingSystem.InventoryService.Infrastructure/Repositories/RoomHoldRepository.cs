using HotelBookingSystem.InventoryService.Application.Interfaces;
using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.InventoryService.Infrastructure.Repositories
{
    public class RoomHoldRepository : IRoomHoldRepository
    {
        private readonly InventoryDbContext _context;

        public RoomHoldRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<RoomHold?> GetActiveHoldByBookingIdAsync(Guid bookingId)
        {
            return await _context.RoomHolds
                .FirstOrDefaultAsync(rh => rh.BookingId == bookingId && !rh.IsReleased);
        }

        public async Task<RoomHold?> GetByHoldReferenceAsync(string holdReference)
        {
            return await _context.RoomHolds
                .FirstOrDefaultAsync(rh => rh.HoldReference == holdReference);
        }

        public async Task CreateAsync(RoomHold roomHold)
        {
            _context.RoomHolds.Add(roomHold);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RoomHold roomHold)
        {
            _context.RoomHolds.Update(roomHold);
            await _context.SaveChangesAsync();
        }
    }
} 