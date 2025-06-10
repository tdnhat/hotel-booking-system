using HotelBookingSystem.BookingService.Domain.Messages.Commands;
using HotelBookingSystem.BookingService.Domain.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.BookingService.Infrastructure.Consumers
{
    public class ProcessPaymentConsumer : IConsumer<ProcessPayment>
    {
        private readonly ILogger<ProcessPaymentConsumer> _logger;

        public ProcessPaymentConsumer(ILogger<ProcessPaymentConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProcessPayment> context)
        {
            var command = context.Message;

            _logger.LogInformation("Processing payment for Booking: {BookingId}, Amount: {Amount} {Currency}", 
                command.BookingId, command.Amount, command.Currency);

            // TODO: Implement payment processing logic
            try
            {
                await Task.Delay(2000, context.CancellationToken);

                var random = new Random();
                var success = random.NextDouble() < 0.8; // 80% chance of success

                if (success)
                {
                    var paymentReference = $"PAY-{command.BookingId.ToString("N")[..8]}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                    _logger.LogInformation("Payment for Booking: {BookingId} successful. Reference: {PaymentReference}", 
                        command.BookingId, paymentReference);

                    // Publish success event
                    await context.Publish<PaymentSucceeded>(new
                    {
                        BookingId = command.BookingId,
                        PaymentReference = paymentReference,
                        AmountPaid = command.Amount,
                        Currency = command.Currency,
                        ProcessedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    _logger.LogError("Payment for Booking: {BookingId} failed", command.BookingId);

                    // Publish failure event
                    await context.Publish<PaymentFailed>(new
                    {
                        BookingId = command.BookingId,
                        Reason = "Payment failed - insufficient funds or card declined",
                        FailedAt = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for Booking: {BookingId}", command.BookingId);

                // Publish failure event
                await context.Publish<PaymentFailed>(new
                {
                    BookingId = command.BookingId,
                    Reason = $"Payment failed: {ex.Message}",
                    FailedAt = DateTime.UtcNow
                });
            }
        }

    }
}