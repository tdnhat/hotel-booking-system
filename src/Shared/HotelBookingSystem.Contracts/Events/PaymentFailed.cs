namespace HotelBookingSystem.Contracts.Events
{
    /// <summary>
    /// Event indicating that a payment has failed.
    /// </summary>
    public class PaymentFailed
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime FailedAt { get; set; }
    }
} 