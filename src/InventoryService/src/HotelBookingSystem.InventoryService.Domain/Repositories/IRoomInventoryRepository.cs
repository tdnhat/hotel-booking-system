using HotelBookingSystem.InventoryService.Domain.Entities;

namespace HotelBookingSystem.InventoryService.Domain.Repositories;

/// <summary>
/// Repository interface for RoomInventory entity operations.
/// </summary>
public interface IRoomInventoryRepository
{
    /// <summary>
    /// Gets a room inventory by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the room inventory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room inventory if found, null otherwise.</returns>
    Task<RoomInventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets room inventories by hotel and room type.
    /// </summary>
    /// <param name="hotelId">The unique identifier of the hotel.</param>
    /// <param name="roomTypeId">The unique identifier of the room type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of room inventories matching the criteria.</returns>
    Task<IEnumerable<RoomInventory>> GetByHotelAndRoomTypeAsync(Guid hotelId, Guid roomTypeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all room inventories for a specific hotel.
    /// </summary>
    /// <param name="hotelId">The unique identifier of the hotel.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of room inventories for the hotel.</returns>
    Task<IEnumerable<RoomInventory>> GetByHotelAsync(Guid hotelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets room inventories with availability data for the specified date range.
    /// </summary>
    /// <param name="hotelId">The unique identifier of the hotel.</param>
    /// <param name="roomTypeId">The unique identifier of the room type.</param>
    /// <param name="checkInDate">The check-in date.</param>
    /// <param name="checkOutDate">The check-out date.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of room inventories with availability data loaded.</returns>
    Task<IEnumerable<RoomInventory>> GetWithAvailabilityAsync(
        Guid hotelId, 
        Guid roomTypeId, 
        DateTime checkInDate, 
        DateTime checkOutDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new room inventory.
    /// </summary>
    /// <param name="roomInventory">The room inventory to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added room inventory.</returns>
    Task<RoomInventory> AddAsync(RoomInventory roomInventory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing room inventory.
    /// </summary>
    /// <param name="roomInventory">The room inventory to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated room inventory.</returns>
    Task<RoomInventory> UpdateAsync(RoomInventory roomInventory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a room inventory.
    /// </summary>
    /// <param name="id">The unique identifier of the room inventory to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a room inventory exists for the specified hotel and room type.
    /// </summary>
    /// <param name="hotelId">The unique identifier of the hotel.</param>
    /// <param name="roomTypeId">The unique identifier of the room type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the room inventory exists, false otherwise.</returns>
    Task<bool> ExistsAsync(Guid hotelId, Guid roomTypeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets room inventories that need availability calendar initialization.
    /// </summary>
    /// <param name="fromDate">The start date for availability calendar initialization.</param>
    /// <param name="toDate">The end date for availability calendar initialization.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of room inventories missing availability data for the date range.</returns>
    Task<IEnumerable<RoomInventory>> GetInventoriesNeedingAvailabilityInitializationAsync(
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);
} 