using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;

namespace HotelBookingSystem.InventoryService.Domain.Repositories;

/// <summary>
/// Repository interface for RoomHold entity operations.
/// </summary>
public interface IRoomHoldRepository
{
    /// <summary>
    /// Gets a room hold by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the room hold.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room hold if found, null otherwise.</returns>
    Task<RoomHold?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a room hold by its hold reference.
    /// </summary>
    /// <param name="holdReference">The hold reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room hold if found, null otherwise.</returns>
    Task<RoomHold?> GetByHoldReferenceAsync(HoldReference holdReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a room hold by booking identifier.
    /// </summary>
    /// <param name="bookingId">The unique identifier of the booking.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room hold if found, null otherwise.</returns>
    Task<RoomHold?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all room holds for a specific room inventory.
    /// </summary>
    /// <param name="roomInventoryId">The unique identifier of the room inventory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of room holds for the room inventory.</returns>
    Task<IEnumerable<RoomHold>> GetByRoomInventoryIdAsync(Guid roomInventoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets room holds by status.
    /// </summary>
    /// <param name="status">The status to filter by.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of room holds with the specified status.</returns>
    Task<IEnumerable<RoomHold>> GetByStatusAsync(RoomHoldStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expired room holds that need to be processed.
    /// </summary>
    /// <param name="currentTime">The current time to check against (default: DateTime.UtcNow).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of expired room holds.</returns>
    Task<IEnumerable<RoomHold>> GetExpiredHoldsAsync(DateTime? currentTime = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active room holds for a specific date range and room inventory.
    /// </summary>
    /// <param name="roomInventoryId">The unique identifier of the room inventory.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of active room holds within the date range.</returns>
    Task<IEnumerable<RoomHold>> GetActiveHoldsForDateRangeAsync(
        Guid roomInventoryId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets room holds that will expire within the specified time window.
    /// </summary>
    /// <param name="withinMinutes">The number of minutes to look ahead for expiring holds.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of room holds expiring soon.</returns>
    Task<IEnumerable<RoomHold>> GetHoldsExpiringWithinAsync(int withinMinutes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new room hold.
    /// </summary>
    /// <param name="roomHold">The room hold to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added room hold.</returns>
    Task<RoomHold> AddAsync(RoomHold roomHold, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing room hold.
    /// </summary>
    /// <param name="roomHold">The room hold to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated room hold.</returns>
    Task<RoomHold> UpdateAsync(RoomHold roomHold, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates multiple room holds in a single transaction.
    /// </summary>
    /// <param name="roomHolds">The collection of room holds to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection of updated room holds.</returns>
    Task<IEnumerable<RoomHold>> UpdateManyAsync(IEnumerable<RoomHold> roomHolds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a room hold.
    /// </summary>
    /// <param name="id">The unique identifier of the room hold to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a room hold exists with the specified hold reference.
    /// </summary>
    /// <param name="holdReference">The hold reference to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the room hold exists, false otherwise.</returns>
    Task<bool> ExistsAsync(HoldReference holdReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of active holds for a specific room inventory.
    /// </summary>
    /// <param name="roomInventoryId">The unique identifier of the room inventory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of active holds.</returns>
    Task<int> GetActiveHoldCountAsync(Guid roomInventoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics for room holds within a date range.
    /// </summary>
    /// <param name="startDate">The start date for statistics.</param>
    /// <param name="endDate">The end date for statistics.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A dictionary containing hold statistics by status.</returns>
    Task<Dictionary<RoomHoldStatus, int>> GetHoldStatisticsAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);
} 