namespace HotelBookingSystem.BookingService.Domain.Messages.Events
{
    /// <summary>
    /// Command to request a booking.
    /// </summary>
    public class BookingRequested
    {
        public Guid BookingId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid HotelId { get; set; }
        public string GuestEmail { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime RequestAt { get; set; } = DateTime.UtcNow;
    }
}