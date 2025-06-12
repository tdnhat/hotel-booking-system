namespace HotelBookingSystem.Contracts.Events
{
    /// <summary>
    /// Event indicating that a room has been released.
    /// </summary>
    public class RoomReleased
    {
        public Guid BookingId { get; set; }
        public string RoomHoldReference { get; set; } = string.Empty;
        public DateTime ReleasedAt { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
} 