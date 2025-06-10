namespace HotelBookingSystem.BookingService.Domain.Messages.Commands
{
    /// <summary>
    /// Command to hold a room for a booking.
    /// </summary>
    public class HoldRoom
    {
        public Guid BookingId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public TimeSpan HoldDuration { get; set; } = TimeSpan.FromMinutes(10); // Hold for 10 minutes by default
    }
}