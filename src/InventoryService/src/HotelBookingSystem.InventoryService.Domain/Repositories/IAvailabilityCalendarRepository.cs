using HotelBookingSystem.InventoryService.Domain.Entities;

namespace HotelBookingSystem.InventoryService.Domain.Repositories;

/// <summary>
/// Repository interface for AvailabilityCalendar entity operations.
/// </summary>
public interface IAvailabilityCalendarRepository
{
    /// <summary>
    /// Gets an availability calendar entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the availability calendar.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The availability calendar if found, null otherwise.</returns>
    Task<AvailabilityCalendar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets availability calendar entries for a specific room inventory and date.
    /// </summary>
    /// <param name="roomInventoryId">The unique identifier of the room inventory.</param>
    /// <param name="availabilityDate">The specific date to get availability for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The availability calendar for the date if found, null otherwise.</returns>
    Task<AvailabilityCalendar?> GetByRoomInventoryAndDateAsync(
        Guid roomInventoryId, 
        DateTime availabilityDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets availability calendar entries for a specific room inventory and date range.
    /// </summary>
    /// <param name="roomInventoryId">The unique identifier of the room inventory.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of availability calendars for the date range.</returns>
    Task<IEnumerable<AvailabilityCalendar>> GetByRoomInventoryAndDateRangeAsync(
        Guid roomInventoryId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets availability calendar entries for multiple room inventories and a date range.
    /// </summary>
    /// <param name="roomInventoryIds">The collection of room inventory identifiers.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of availability calendars for the room inventories and date range.</returns>
    Task<IEnumerable<AvailabilityCalendar>> GetByRoomInventoriesAndDateRangeAsync(
        IEnumerable<Guid> roomInventoryIds, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets availability calendar entries that have available rooms for a specific date range.
    /// </summary>
    /// <param name="hotelId">The unique identifier of the hotel.</param>
    /// <param name="roomTypeId">The unique identifier of the room type.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="minimumAvailable">The minimum number of available rooms required.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of availability calendars with sufficient availability.</returns>
    Task<IEnumerable<AvailabilityCalendar>> GetAvailableForDateRangeAsync(
        Guid hotelId, 
        Guid roomTypeId, 
        DateTime startDate, 
        DateTime endDate, 
        int minimumAvailable = 1, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets dates that are missing availability calendar entries for a room inventory.
    /// </summary>
    /// <param name="roomInventoryId">The unique identifier of the room inventory.</param>
    /// <param name="startDate">The start date of the range to check.</param>
    /// <param name="endDate">The end date of the range to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of dates missing availability calendar entries.</returns>
    Task<IEnumerable<DateTime>> GetMissingDatesAsync(
        Guid roomInventoryId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets availability calendar entries that were last updated before the specified time.
    /// </summary>
    /// <param name="updatedBefore">The cutoff time for last update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of availability calendars needing updates.</returns>
    Task<IEnumerable<AvailabilityCalendar>> GetStaleEntriesAsync(
        DateTime updatedBefore, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new availability calendar entry.
    /// </summary>
    /// <param name="availabilityCalendar">The availability calendar to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added availability calendar.</returns>
    Task<AvailabilityCalendar> AddAsync(AvailabilityCalendar availabilityCalendar, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple availability calendar entries in a single transaction.
    /// </summary>
    /// <param name="availabilityCalendars">The collection of availability calendars to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection of added availability calendars.</returns>
    Task<IEnumerable<AvailabilityCalendar>> AddManyAsync(
        IEnumerable<AvailabilityCalendar> availabilityCalendars, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing availability calendar entry.
    /// </summary>
    /// <param name="availabilityCalendar">The availability calendar to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated availability calendar.</returns>
    Task<AvailabilityCalendar> UpdateAsync(AvailabilityCalendar availabilityCalendar, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates multiple availability calendar entries in a single transaction.
    /// </summary>
    /// <param name="availabilityCalendars">The collection of availability calendars to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection of updated availability calendars.</returns>
    Task<IEnumerable<AvailabilityCalendar>> UpdateManyAsync(
        IEnumerable<AvailabilityCalendar> availabilityCalendars, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates availability calendar entries for a date range.
    /// </summary>
    /// <param name="roomInventoryId">The unique identifier of the room inventory.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="totalRooms">The total number of rooms in the inventory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection of created or updated availability calendars.</returns>
    Task<IEnumerable<AvailabilityCalendar>> UpsertDateRangeAsync(
        Guid roomInventoryId, 
        DateTime startDate, 
        DateTime endDate, 
        int totalRooms, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an availability calendar entry.
    /// </summary>
    /// <param name="id">The unique identifier of the availability calendar to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes availability calendar entries older than the specified date.
    /// </summary>
    /// <param name="olderThan">The cutoff date for deletion.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entries deleted.</returns>
    Task<int> DeleteOldEntriesAsync(DateTime olderThan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if availability calendar entries exist for the specified room inventory and date range.
    /// </summary>
    /// <param name="roomInventoryId">The unique identifier of the room inventory.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if all dates have availability calendar entries, false otherwise.</returns>
    Task<bool> ExistsForDateRangeAsync(
        Guid roomInventoryId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets aggregated availability statistics for a room inventory and date range.
    /// </summary>
    /// <param name="roomInventoryId">The unique identifier of the room inventory.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A dictionary containing availability statistics.</returns>
    Task<Dictionary<string, object>> GetAvailabilityStatisticsAsync(
        Guid roomInventoryId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);
} 