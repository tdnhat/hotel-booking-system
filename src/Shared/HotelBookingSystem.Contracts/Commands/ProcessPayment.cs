namespace HotelBookingSystem.Contracts.Commands
{
    /// <summary>
    /// Command to process a payment for a booking.
    /// </summary>
    public class ProcessPayment
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string PaymentMethod { get; set; } = string.Empty;
        public string RoomHoldReference { get; set; } = string.Empty;
    }
} 