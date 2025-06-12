namespace HotelBookingSystem.Contracts.Events
{
    /// <summary>
    /// Event indicating that a payment has been successfully processed.
    /// </summary>
    public class PaymentSucceeded
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
    }
} 