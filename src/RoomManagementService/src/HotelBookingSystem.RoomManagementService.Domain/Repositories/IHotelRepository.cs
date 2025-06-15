using HotelBookingSystem.RoomManagementService.Domain.Entities;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;

namespace HotelBookingSystem.RoomManagementService.Domain.Repositories
{
    public interface IHotelRepository
    {
        Task<Hotel?> GetByIdAsync(HotelId id, CancellationToken cancellationToken = default);

        Task<Hotel?> GetByIdWithRoomTypesAsync(HotelId id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Hotel>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Hotel>> GetActiveHotelsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Hotel>> GetHotelsByLocationAsync(string city, string state, string country, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(HotelId id, CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

        Task AddAsync(Hotel hotel, CancellationToken cancellationToken = default);

        Task UpdateAsync(Hotel hotel, CancellationToken cancellationToken = default);

        Task DeleteAsync(HotelId id, CancellationToken cancellationToken = default);

        Task<RoomType?> GetRoomTypeAsync(RoomTypeId roomTypeId, CancellationToken cancellationToken = default);

        Task<IEnumerable<RoomType>> GetRoomTypesByHotelAsync(HotelId hotelId, CancellationToken cancellationToken = default);
    }
}
