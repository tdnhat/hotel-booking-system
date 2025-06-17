using HotelBookingSystem.InventoryService.Application.Interfaces;
using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Domain.Repositories;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;
using HotelBookingSystem.InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.InventoryService.Infrastructure.Repositories;

public class RoomHoldRepository : Domain.Repositories.IRoomHoldRepository
{
    private readonly IInventoryDbContext _context;

    public RoomHoldRepository(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<RoomHold?> GetByIdAsync(HoldId id, CancellationToken cancellationToken = default)
    {
        return await _context.RoomHolds
            .FirstOrDefaultAsync(rh => rh.Id == id, cancellationToken);
    }

        public async Task<RoomHold?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await _context.RoomHolds
            .FirstOrDefaultAsync(rh => rh.BookingId == bookingId, cancellationToken);
    }

    public async Task<RoomHold?> GetByHoldReferenceAsync(string holdReference, CancellationToken cancellationToken = default)
    {
        return await _context.RoomHolds
            .FirstOrDefaultAsync(rh => rh.HoldReference == holdReference, cancellationToken);
    }

    public async Task<IEnumerable<RoomHold>> GetActiveHoldsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RoomHolds
            .Where(rh => rh.Status == HoldStatus.Active && rh.ExpiresAt > DateTime.UtcNow)
            .OrderBy(rh => rh.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RoomHold>> GetExpiredHoldsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RoomHolds
            .Where(rh => rh.Status == HoldStatus.Active && rh.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RoomHold>> GetHoldsByStatusAsync(
        HoldStatus status, 
        CancellationToken cancellationToken = default)
    {
        return await _context.RoomHolds
            .Where(rh => rh.Status == status)
            .OrderBy(rh => rh.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RoomHold>> GetHoldsByRoomTypeAsync(
        HotelId hotelId,
        RoomTypeId roomTypeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RoomHolds
            .Where(rh => rh.HotelId == hotelId && rh.RoomTypeId == roomTypeId)
            .OrderBy(rh => rh.CreatedAt)
            .ToListAsync(cancellationToken);
    }



    public void Add(RoomHold roomHold)
    {
        _context.RoomHolds.Add(roomHold);
    }

    public void Update(RoomHold roomHold)
    {
        _context.RoomHolds.Update(roomHold);
    }

    public void Remove(RoomHold roomHold)
    {
        _context.RoomHolds.Remove(roomHold);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
} 