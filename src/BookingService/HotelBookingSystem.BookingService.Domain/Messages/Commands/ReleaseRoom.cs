namespace HotelBookingSystem.BookingService.Domain.Messages.Commands
{
    /// <summary>
    /// Command to indicate that a room has been released.
    /// </summary>
    public class ReleaseRoom
    {
        public Guid BookingId { get; set; }
        public string RoomHoldReference { get; set; } = string.Empty;
        public DateTime ReleasedAt { get; set; } = DateTime.UtcNow;
    }
}