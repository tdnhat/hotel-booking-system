using HotelBookingSystem.RoomManagementService.Domain.Entities;
using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Repositories
{
    public class RoomTypeRepository : BaseRepository<RoomType>, IRoomTypeRepository
    {
        public RoomTypeRepository(IRoomManagementDbContext context) : base(context)
        {
        }

        public override async Task<RoomType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(rt => rt.Rooms)
                .FirstOrDefaultAsync(rt => rt.Id == id, cancellationToken);
        }

        public override async Task<IEnumerable<RoomType>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(rt => rt.Rooms)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RoomType>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(rt => rt.Rooms)
                .Where(rt => rt.HotelId == hotelId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsNameUniqueAsync(string name, Guid hotelId, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(rt => rt.Name == name && rt.HotelId == hotelId);
            
            if (excludeId.HasValue)
            {
                query = query.Where(rt => rt.Id != excludeId.Value);
            }
            
            return !await query.AnyAsync(cancellationToken);
        }
    }
}
