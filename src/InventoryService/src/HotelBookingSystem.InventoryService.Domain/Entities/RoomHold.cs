using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;

namespace HotelBookingSystem.InventoryService.Domain.Entities;

/// <summary>
/// Represents a temporary hold on a room for a specific booking.
/// Holds automatically expire after a specified duration.
/// </summary>
public class RoomHold
{
    /// <summary>
    /// Gets or sets the unique identifier for the room hold.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the booking this hold is for.
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the room inventory being held.
    /// </summary>
    public Guid RoomInventoryId { get; set; }

    /// <summary>
    /// Gets or sets the unique reference for this hold.
    /// </summary>
    public HoldReference HoldReference { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp when the hold was created.
    /// </summary>
    public DateTime HeldAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the hold expires.
    /// </summary>
    public DateTime HeldUntil { get; set; }

    /// <summary>
    /// Gets or sets the current status of the room hold.
    /// </summary>
    public RoomHoldStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the identifier of who created this hold.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when this hold was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this hold was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the room inventory associated with this hold.
    /// </summary>
    public virtual RoomInventory RoomInventory { get; set; } = null!;

    /// <summary>
    /// Checks if the room hold has expired.
    /// </summary>
    /// <param name="currentTime">The current time to check against. If null, uses DateTime.UtcNow.</param>
    /// <returns>True if the hold has expired, false otherwise.</returns>
    public bool IsExpired(DateTime? currentTime = null)
    {
        var checkTime = currentTime ?? DateTime.UtcNow;
        return Status == RoomHoldStatus.Active && HeldUntil <= checkTime;
    }

    /// <summary>
    /// Releases the room hold, making the room available again.
    /// </summary>
    /// <param name="releasedBy">The identifier of who is releasing the hold.</param>
    /// <returns>True if the hold was successfully released, false if it was already released or expired.</returns>
    public bool Release(string releasedBy = "System")
    {
        if (Status != RoomHoldStatus.Active)
            return false;

        Status = RoomHoldStatus.Released;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Extends the hold duration by the specified time span.
    /// </summary>
    /// <param name="additionalTime">The additional time to extend the hold.</param>
    /// <param name="maxExtensionTime">The maximum total extension time allowed.</param>
    /// <returns>True if the hold was successfully extended, false otherwise.</returns>
    public bool Extend(TimeSpan additionalTime, TimeSpan? maxExtensionTime = null)
    {
        if (Status != RoomHoldStatus.Active)
            return false;

        if (IsExpired())
            return false;

        var originalDuration = HeldUntil - HeldAt;
        var newDuration = originalDuration + additionalTime;

        // Check if extension exceeds maximum allowed time
        if (maxExtensionTime.HasValue && newDuration > maxExtensionTime.Value)
            return false;

        HeldUntil = HeldUntil.Add(additionalTime);
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Confirms the room hold, typically when payment is successful.
    /// </summary>
    /// <returns>True if the hold was successfully confirmed, false otherwise.</returns>
    public bool Confirm()
    {
        if (Status != RoomHoldStatus.Active)
            return false;

        if (IsExpired())
            return false;

        Status = RoomHoldStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Marks the hold as expired. This is typically called by a background service.
    /// </summary>
    /// <returns>True if the hold was successfully marked as expired, false otherwise.</returns>
    public bool MarkAsExpired()
    {
        if (Status != RoomHoldStatus.Active)
            return false;

        Status = RoomHoldStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Validates the room hold for domain consistency.
    /// </summary>
    /// <returns>A list of validation errors, empty if valid.</returns>
    public List<string> Validate()
    {
        var errors = new List<string>();

        if (BookingId == Guid.Empty)
            errors.Add("BookingId cannot be empty.");

        if (RoomInventoryId == Guid.Empty)
            errors.Add("RoomInventoryId cannot be empty.");

        if (HoldReference == null)
            errors.Add("HoldReference cannot be null.");

        if (HeldAt == default)
            errors.Add("HeldAt must be set.");

        if (HeldUntil <= HeldAt)
            errors.Add("HeldUntil must be after HeldAt.");

        if (string.IsNullOrWhiteSpace(CreatedBy))
            errors.Add("CreatedBy cannot be empty.");

        return errors;
    }
} 