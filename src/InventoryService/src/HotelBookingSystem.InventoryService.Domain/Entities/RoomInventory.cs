namespace HotelBookingSystem.InventoryService.Domain.Entities;

/// <summary>
/// Represents the inventory of rooms for a specific room type in a hotel.
/// Manages room availability and tracks holds and bookings.
/// </summary>
public class RoomInventory
{
    /// <summary>
    /// Gets or sets the unique identifier for the room inventory.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the hotel this inventory belongs to.
    /// </summary>
    public Guid HotelId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the room type.
    /// </summary>
    public Guid RoomTypeId { get; set; }

    /// <summary>
    /// Gets or sets the room number or identifier.
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of rooms available for this room type.
    /// </summary>
    public int TotalRooms { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this inventory was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this inventory was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the collection of room holds associated with this inventory.
    /// </summary>
    public virtual ICollection<RoomHold> RoomHolds { get; set; } = new List<RoomHold>();

    /// <summary>
    /// Gets or sets the collection of availability calendar entries for this inventory.
    /// </summary>
    public virtual ICollection<AvailabilityCalendar> AvailabilityCalendars { get; set; } = new List<AvailabilityCalendar>();

    /// <summary>
    /// Checks if rooms are available for the specified date range.
    /// </summary>
    /// <param name="checkInDate">The check-in date.</param>
    /// <param name="checkOutDate">The check-out date.</param>
    /// <param name="numberOfRooms">The number of rooms requested.</param>
    /// <returns>True if the requested number of rooms are available for the entire date range.</returns>
    public bool CheckAvailability(DateTime checkInDate, DateTime checkOutDate, int numberOfRooms = 1)
    {
        if (checkInDate >= checkOutDate)
            return false;

        if (numberOfRooms <= 0 || numberOfRooms > TotalRooms)
            return false;

        // Check each date in the range for availability
        for (var date = checkInDate.Date; date < checkOutDate.Date; date = date.AddDays(1))
        {
            var availabilityForDate = AvailabilityCalendars
                .FirstOrDefault(ac => ac.AvailabilityDate.Date == date);

            if (availabilityForDate == null)
            {
                // If no availability record exists, assume all rooms are available
                if (numberOfRooms > TotalRooms)
                    return false;
            }
            else
            {
                var availableRooms = availabilityForDate.AvailableRooms;
                if (availableRooms < numberOfRooms)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the number of available rooms for the specified date range.
    /// </summary>
    /// <param name="checkInDate">The check-in date.</param>
    /// <param name="checkOutDate">The check-out date.</param>
    /// <returns>The minimum number of available rooms across the date range.</returns>
    public int GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate)
    {
        if (checkInDate >= checkOutDate)
            return 0;

        var minAvailable = TotalRooms;

        for (var date = checkInDate.Date; date < checkOutDate.Date; date = date.AddDays(1))
        {
            var availabilityForDate = AvailabilityCalendars
                .FirstOrDefault(ac => ac.AvailabilityDate.Date == date);

            if (availabilityForDate == null)
            {
                // If no availability record exists, assume all rooms are available
                continue;
            }

            minAvailable = Math.Min(minAvailable, availabilityForDate.AvailableRooms);
        }

        return Math.Max(0, minAvailable);
    }
}