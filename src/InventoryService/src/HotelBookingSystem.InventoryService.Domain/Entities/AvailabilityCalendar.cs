namespace HotelBookingSystem.InventoryService.Domain.Entities;

/// <summary>
/// Represents the availability status of rooms for a specific date.
/// Tracks available, held, and booked rooms for inventory management.
/// </summary>
public class AvailabilityCalendar
{
    /// <summary>
    /// Gets or sets the unique identifier for the availability calendar entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the room inventory this calendar entry belongs to.
    /// </summary>
    public Guid RoomInventoryId { get; set; }

    /// <summary>
    /// Gets or sets the specific date for this availability entry.
    /// </summary>
    public DateTime AvailabilityDate { get; set; }

    /// <summary>
    /// Gets or sets the number of rooms currently available for booking.
    /// </summary>
    public int AvailableRooms { get; set; }

    /// <summary>
    /// Gets or sets the number of rooms currently held (temporarily reserved).
    /// </summary>
    public int HeldRooms { get; set; }

    /// <summary>
    /// Gets or sets the number of rooms that are confirmed booked.
    /// </summary>
    public int BookedRooms { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this availability was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the room inventory associated with this availability calendar.
    /// </summary>
    public virtual RoomInventory RoomInventory { get; set; } = null!;

    /// <summary>
    /// Updates the availability counts ensuring consistency.
    /// </summary>
    /// <param name="availableRooms">The new number of available rooms.</param>
    /// <param name="heldRooms">The new number of held rooms.</param>
    /// <param name="bookedRooms">The new number of booked rooms.</param>
    /// <returns>True if the update was successful, false if validation failed.</returns>
    public bool UpdateAvailability(int availableRooms, int heldRooms, int bookedRooms)
    {
        if (availableRooms < 0 || heldRooms < 0 || bookedRooms < 0)
            return false;

        // The total should not exceed the total rooms in the inventory
        var totalUsed = availableRooms + heldRooms + bookedRooms;
        if (RoomInventory != null && totalUsed > RoomInventory.TotalRooms)
            return false;

        AvailableRooms = availableRooms;
        HeldRooms = heldRooms;
        BookedRooms = bookedRooms;
        LastUpdated = DateTime.UtcNow;

        return true;
    }

    /// <summary>
    /// Calculates the number of available rooms based on total inventory and current bookings/holds.
    /// </summary>
    /// <param name="totalInventoryRooms">The total number of rooms in the inventory.</param>
    /// <returns>The calculated number of available rooms.</returns>
    public int CalculateAvailable(int totalInventoryRooms)
    {
        var calculated = totalInventoryRooms - HeldRooms - BookedRooms;
        return Math.Max(0, calculated);
    }

    /// <summary>
    /// Holds a specified number of rooms, reducing availability.
    /// </summary>
    /// <param name="numberOfRooms">The number of rooms to hold.</param>
    /// <returns>True if the hold was successful, false if not enough rooms available.</returns>
    public bool HoldRooms(int numberOfRooms)
    {
        if (numberOfRooms <= 0)
            return false;

        if (AvailableRooms < numberOfRooms)
            return false;

        AvailableRooms -= numberOfRooms;
        HeldRooms += numberOfRooms;
        LastUpdated = DateTime.UtcNow;

        return true;
    }

    /// <summary>
    /// Releases held rooms back to available inventory.
    /// </summary>
    /// <param name="numberOfRooms">The number of rooms to release.</param>
    /// <returns>True if the release was successful, false if validation failed.</returns>
    public bool ReleaseHeldRooms(int numberOfRooms)
    {
        if (numberOfRooms <= 0)
            return false;

        if (HeldRooms < numberOfRooms)
            return false;

        HeldRooms -= numberOfRooms;
        AvailableRooms += numberOfRooms;
        LastUpdated = DateTime.UtcNow;

        return true;
    }

    /// <summary>
    /// Confirms a hold by converting held rooms to booked rooms.
    /// </summary>
    /// <param name="numberOfRooms">The number of rooms to confirm.</param>
    /// <returns>True if the confirmation was successful, false if validation failed.</returns>
    public bool ConfirmHeldRooms(int numberOfRooms)
    {
        if (numberOfRooms <= 0)
            return false;

        if (HeldRooms < numberOfRooms)
            return false;

        HeldRooms -= numberOfRooms;
        BookedRooms += numberOfRooms;
        LastUpdated = DateTime.UtcNow;

        return true;
    }

    /// <summary>
    /// Validates the availability calendar for domain consistency.
    /// </summary>
    /// <returns>A list of validation errors, empty if valid.</returns>
    public List<string> Validate()
    {
        var errors = new List<string>();

        if (RoomInventoryId == Guid.Empty)
            errors.Add("RoomInventoryId cannot be empty.");

        if (AvailabilityDate == default)
            errors.Add("AvailabilityDate must be set.");

        if (AvailableRooms < 0)
            errors.Add("AvailableRooms cannot be negative.");

        if (HeldRooms < 0)
            errors.Add("HeldRooms cannot be negative.");

        if (BookedRooms < 0)
            errors.Add("BookedRooms cannot be negative.");

        return errors;
    }

    /// <summary>
    /// Gets the total number of rooms that are not available (held + booked).
    /// </summary>
    /// <returns>The total number of unavailable rooms.</returns>
    public int GetUnavailableRooms()
    {
        return HeldRooms + BookedRooms;
    }

    /// <summary>
    /// Checks if the specified number of rooms can be held.
    /// </summary>
    /// <param name="numberOfRooms">The number of rooms to check.</param>
    /// <returns>True if the rooms can be held, false otherwise.</returns>
    public bool CanHoldRooms(int numberOfRooms)
    {
        return numberOfRooms > 0 && AvailableRooms >= numberOfRooms;
    }
} 