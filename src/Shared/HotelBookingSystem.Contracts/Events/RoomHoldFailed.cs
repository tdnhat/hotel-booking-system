namespace HotelBookingSystem.Contracts.Events
{
    /// <summary>
    /// Event indicating that a room hold request has failed.
    /// </summary>
    public class RoomHoldFailed
    {
        public Guid BookingId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime FailedAt { get; set; }
    }
} 