namespace HotelBookingSystem.BookingService.Domain.Messages.Events
{
    /// <summary>
    /// Command to indicate that room hold failed.
    /// </summary>
    public class RoomHoldFailed
    {
        public Guid BookingId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime FailedAt { get; set; } = DateTime.UtcNow;
    }
}