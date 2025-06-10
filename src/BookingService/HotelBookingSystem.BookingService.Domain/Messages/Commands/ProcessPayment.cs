namespace HotelBookingSystem.BookingService.Domain.Messages.Commands
{
    /// <summary>
    /// Command to process a payment.
    /// </summary>
    public class ProcessPayment
    {
        public Guid BookingId { get; set; }
        public string GuestEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? PaymentMethod { get; set; } = "CreditCard";
    }
}