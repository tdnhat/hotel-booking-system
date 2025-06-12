namespace HotelBookingSystem.Contracts.Events
{
    /// <summary>
    /// Event indicating that a room has been successfully held.
    /// </summary>
    public class RoomHeld
    {
        public Guid BookingId { get; set; }
        public string RoomHoldReference { get; set; } = string.Empty;
        public DateTime HeldUntil { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
    }
} 