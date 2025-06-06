using HotelBookingSystem.RoomManagementService.Domain.Entities;

namespace HotelBookingSystem.RoomManagementService.Domain.Repositories
{
    public interface IRoomTypeRepository : IBaseRepository<RoomType>
    {
        Task<IEnumerable<RoomType>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
        Task<bool> IsNameUniqueAsync(string name, Guid hotelId, Guid? excludeId = null, CancellationToken cancellationToken = default);
    }
}
