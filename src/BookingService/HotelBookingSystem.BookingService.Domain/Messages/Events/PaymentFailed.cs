namespace HotelBookingSystem.BookingService.Domain.Messages.Events
{
    /// <summary>
    /// Command to indicate that a payment failed.
    /// </summary>
    public class PaymentFailed
    {
        public Guid BookingId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime FailedAt { get; set; } = DateTime.UtcNow;
    }
}