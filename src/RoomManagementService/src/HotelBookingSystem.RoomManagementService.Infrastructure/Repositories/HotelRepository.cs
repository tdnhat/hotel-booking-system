using HotelBookingSystem.RoomManagementService.Domain.Entities;
using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;
using HotelBookingSystem.RoomManagementService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly RoomManagementDbContext _context;

        public HotelRepository(RoomManagementDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Hotel?> GetByIdAsync(HotelId id, CancellationToken cancellationToken = default)
        {
            return await _context.Hotels
                .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        }

        public async Task<Hotel?> GetByIdWithRoomTypesAsync(HotelId id, CancellationToken cancellationToken = default)
        {
            return await _context.Hotels
                .Include(h => h.RoomTypes)
                .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Hotel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Hotels
                .OrderBy(h => h.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Hotel>> GetActiveHotelsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Hotels
                .Where(h => h.Status == Domain.Enums.HotelStatus.Active)
                .OrderBy(h => h.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Hotel>> GetHotelsByLocationAsync(string city, string state, string country, CancellationToken cancellationToken = default)
        {
            return await _context.Hotels
                .Where(h => h.Address.City == city && 
                           h.Address.State == state && 
                           h.Address.Country == country)
                .OrderBy(h => h.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(HotelId id, CancellationToken cancellationToken = default)
        {
            return await _context.Hotels
                .AnyAsync(h => h.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Hotels
                .AnyAsync(h => h.Name == name, cancellationToken);
        }

        public async Task AddAsync(Hotel hotel, CancellationToken cancellationToken = default)
        {
            await _context.Hotels.AddAsync(hotel, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Hotel hotel, CancellationToken cancellationToken = default)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(HotelId id, CancellationToken cancellationToken = default)
        {
            var hotel = await GetByIdAsync(id, cancellationToken);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<RoomType?> GetRoomTypeAsync(RoomTypeId roomTypeId, CancellationToken cancellationToken = default)
        {
            return await _context.RoomTypes
                .FirstOrDefaultAsync(rt => rt.Id == roomTypeId, cancellationToken);
        }

        public async Task<IEnumerable<RoomType>> GetRoomTypesByHotelAsync(HotelId hotelId, CancellationToken cancellationToken = default)
        {
            return await _context.RoomTypes
                .Where(rt => rt.HotelId == hotelId)
                .OrderBy(rt => rt.Name)
                .ToListAsync(cancellationToken);
        }
    }
} 