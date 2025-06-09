namespace HotelBookingSystem.BookingService.Domain.Messages.Events
{
    /// <summary>
    /// Event published when a room hold has been successfully released.
    /// </summary>
    public class RoomReleased
    {
        public Guid BookingId { get; set; }
        public string RoomHoldReference { get; set; } = string.Empty;
        public DateTime ReleasedAt { get; set; } = DateTime.UtcNow;
        public string Reason { get; set; } = string.Empty;
    }
} 