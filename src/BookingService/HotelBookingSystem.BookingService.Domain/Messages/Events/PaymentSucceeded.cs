namespace HotelBookingSystem.BookingService.Domain.Messages.Events
{
    /// <summary>
    /// Command to indicate that a payment succeeded.
    /// </summary>
    public class PaymentSucceeded
    {
        public Guid BookingId { get; set; }
        public string PaymentReference { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}