namespace HotelBookingSystem.InventoryService.Domain.Enums;

/// <summary>
/// Represents the status of a room hold throughout its lifecycle.
/// </summary>
public enum RoomHoldStatus
{
    /// <summary>
    /// The room hold is currently active and valid.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The room hold has expired due to timeout.
    /// </summary>
    Expired = 2,

    /// <summary>
    /// The room hold has been manually released.
    /// </summary>
    Released = 3,

    /// <summary>
    /// The room hold has been confirmed (typically after successful payment).
    /// </summary>
    Confirmed = 4
} 