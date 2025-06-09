namespace HotelBookingSystem.BookingService.Domain.Messages.Events
{
    /// <summary>
    /// Command to confirm a room hold.
    /// </summary>
    public class RoomHeld
    {
        public Guid BookingId { get; set; }
        public string RoomHoldReference { get; set; } = string.Empty;
        public DateTime HeldUntil { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
    }
}