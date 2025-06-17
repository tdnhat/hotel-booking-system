using HotelBookingSystem.BookingService.Domain.Entities;
using HotelBookingSystem.Contracts.Commands;
using HotelBookingSystem.BookingService.Domain.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using ContractEvents = HotelBookingSystem.Contracts.Events;

namespace HotelBookingSystem.BookingService.Infrastructure.Saga
{
    /// <summary>
    /// Saga State Machine that orchestrates the booking process
    /// Handles the complete workflow from booking request to confirmation or failure
    /// </summary>
    public class BookingSagaStateMachine : MassTransitStateMachine<BookingState>
    {
        private readonly ILogger<BookingSagaStateMachine> _logger;
        // State declarations
        public State Submitted { get; private set; } = null!;
        public State RoomHoldRequested { get; private set; } = null!;
        public State RoomHeldState { get; private set; } = null!;
        public State PaymentProcessing { get; private set; } = null!;
        public State RoomReleaseRequested { get; private set; } = null!;
        public State Confirmed { get; private set; } = null!;
        public State Failed { get; private set; } = null!;        // Event declarations
        public Event<BookingRequested> BookingRequested { get; private set; } = null!;
        public Event<ContractEvents.RoomHeld> RoomHeld { get; private set; } = null!;
        public Event<ContractEvents.RoomHoldFailed> RoomHoldFailed { get; private set; } = null!;
        public Event<ContractEvents.PaymentSucceeded> PaymentSucceeded { get; private set; } = null!;
        public Event<ContractEvents.PaymentFailed> PaymentFailed { get; private set; } = null!;
        public Event<ContractEvents.RoomReleased> RoomReleased { get; private set; } = null!;

        public BookingSagaStateMachine(ILogger<BookingSagaStateMachine> logger)
        {
            _logger = logger;
            
            // Configure the state machine - specify which property tracks the current state
            InstanceState(x => x.CurrentState);

            ConfigureCorrelation();
            ConfigureWorkflow();
        }

        private void ConfigureCorrelation()
        {
            // Tell the state machine to use the BookingId to correlate the events
            Event(() => BookingRequested, x =>
                x.CorrelateBy((state, context) => state.BookingId == context.Message.BookingId)
                .SelectId(context => context.Message.BookingId) // Use the BookingId as the correlation ID
            );
            Event(() => RoomHeld, x => x.CorrelateBy((state, context) => state.BookingId == context.Message.BookingId));
            Event(() => RoomHoldFailed, x => x.CorrelateBy((state, context) => state.BookingId == context.Message.BookingId));
            Event(() => PaymentSucceeded, x => x.CorrelateBy((state, context) => state.BookingId == context.Message.BookingId));
            Event(() => PaymentFailed, x => x.CorrelateBy((state, context) => state.BookingId == context.Message.BookingId));
            Event(() => RoomReleased, x => x.CorrelateBy((state, context) => state.BookingId == context.Message.BookingId));
        }

        private void ConfigureWorkflow()
        {
            Initially(
                When(BookingRequested)
                    .Then(context =>
                    {
                        // Initialize saga state from the booking request
                        var message = context.Message;
                        context.Saga.CorrelationId = message.BookingId;
                        context.Saga.BookingId = message.BookingId;
                        context.Saga.RoomTypeId = message.RoomTypeId;
                        context.Saga.HotelId = message.HotelId;
                        context.Saga.GuestEmail = message.GuestEmail;
                        context.Saga.TotalPrice = message.TotalPrice;
                        context.Saga.CheckInDate = message.CheckInDate;
                        context.Saga.CheckOutDate = message.CheckOutDate;
                        context.Saga.NumberOfGuests = message.NumberOfGuests;
                        context.Saga.CreatedAt = DateTime.UtcNow;
                        context.Saga.UpdatedAt = DateTime.UtcNow;

                        _logger.LogInformation(
                            "Booking saga initiated for BookingId: {BookingId}, CorrelationId: {CorrelationId}, " +
                            "Hotel: {HotelId}, RoomType: {RoomTypeId}, Guest: {GuestEmail}, " +
                            "CheckIn: {CheckInDate}, CheckOut: {CheckOutDate}, TotalPrice: {TotalPrice}",
                            message.BookingId, context.Saga.CorrelationId, message.HotelId, 
                            message.RoomTypeId, message.GuestEmail, message.CheckInDate, 
                            message.CheckOutDate, message.TotalPrice);
                    })
                    // Send command to hold the room
                    .PublishAsync(context => context.Init<HoldRoom>(new
                    {
                        BookingId = context.Saga.BookingId,
                        RoomTypeId = context.Saga.RoomTypeId,
                        HotelId = context.Saga.HotelId,
                        CheckInDate = context.Saga.CheckInDate,
                        CheckOutDate = context.Saga.CheckOutDate,
                        NumberOfGuests = context.Saga.NumberOfGuests,
                        HoldDuration = TimeSpan.FromMinutes(10)
                    }))
                    .Then(context =>
                    {
                        _logger.LogInformation(
                            "HoldRoom command published for BookingId: {BookingId}, RoomType: {RoomTypeId}, Hotel: {HotelId}",
                            context.Saga.BookingId, context.Saga.RoomTypeId, context.Saga.HotelId);
                    })
                    .TransitionTo(RoomHoldRequested) // Wait for room hold response
            );

            // Room hold responses
            During(RoomHoldRequested,
                When(RoomHeld)
                    .Then(context =>
                    {
                        context.Saga.RoomHoldReference = context.Message.RoomHoldReference;
                        context.Saga.UpdatedAt = DateTime.UtcNow;

                        _logger.LogInformation(
                            "Room hold successful for BookingId: {BookingId}, HoldReference: {RoomHoldReference}, " +
                            "RoomNumber: {RoomNumber}, HeldUntil: {HeldUntil}",
                            context.Saga.BookingId, context.Message.RoomHoldReference,
                            context.Message.RoomNumber, context.Message.HeldUntil);
                    })
                    // Send command to process payment
                    .PublishAsync(context => context.Init<ProcessPayment>(new
                    {
                        BookingId = context.Saga.BookingId,
                        GuestEmail = context.Saga.GuestEmail,
                        Amount = context.Saga.TotalPrice,
                        Currency = "USD",
                        PaymentMethod = "CreditCard"
                    }))
                    .Then(context =>
                    {
                        _logger.LogInformation(
                            "ProcessPayment command published for BookingId: {BookingId}, Amount: {Amount}, Guest: {GuestEmail}",
                            context.Saga.BookingId, context.Saga.TotalPrice, context.Saga.GuestEmail);
                    })
                    .TransitionTo(PaymentProcessing),
                When(RoomHoldFailed)
                    .Then(context =>
                    {
                        context.Saga.FailureReason = $"Room hold failed: {context.Message.Reason}";
                        context.Saga.UpdatedAt = DateTime.UtcNow;

                        _logger.LogWarning(
                            "Room hold failed for BookingId: {BookingId}, Reason: {Reason}, Hotel: {HotelId}, RoomType: {RoomTypeId}",
                            context.Saga.BookingId, context.Message.Reason, context.Saga.HotelId, context.Saga.RoomTypeId);
                    })
                    .TransitionTo(Failed) // Don't finalize - keep for status queries
            );

            // Payment processing responses
            During(PaymentProcessing,                When(PaymentSucceeded)
                    .Then(context =>
                    {
                        context.Saga.PaymentReference = context.Message.TransactionId; // Use TransactionId from contract
                        context.Saga.UpdatedAt = DateTime.UtcNow;

                        _logger.LogInformation(
                            "Payment successful for BookingId: {BookingId}, PaymentReference: {PaymentReference}, " +
                            "Amount: {Amount}, Guest: {GuestEmail}",
                            context.Saga.BookingId, context.Message.TransactionId,
                            context.Saga.TotalPrice, context.Saga.GuestEmail);
                    })
                    .TransitionTo(Confirmed), // Success - keep for status queries
                When(PaymentFailed)
                    .Then(context =>
                    {
                        context.Saga.FailureReason = $"Payment failed: {context.Message.Reason}";
                        context.Saga.UpdatedAt = DateTime.UtcNow;

                        _logger.LogWarning(
                            "Payment failed for BookingId: {BookingId}, Reason: {Reason}, Amount: {Amount}, Guest: {GuestEmail}",
                            context.Saga.BookingId, context.Message.Reason, context.Saga.TotalPrice, context.Saga.GuestEmail);
                    })
                    // Compensate by releasing the room if we have a hold reference
                    .IfElse(context => !string.IsNullOrEmpty(context.Saga.RoomHoldReference),
                        compensate => compensate
                            .Then(context =>
                            {
                                _logger.LogInformation(
                                    "Initiating compensation - releasing room hold for BookingId: {BookingId}, HoldReference: {RoomHoldReference}",
                                    context.Saga.BookingId, context.Saga.RoomHoldReference);
                            })
                            .PublishAsync(context => context.Init<ReleaseRoom>(new
                            {
                                BookingId = context.Saga.BookingId,
                                RoomHoldReference = context.Saga.RoomHoldReference,
                                ReleasedAt = DateTime.UtcNow
                            }))
                            .TransitionTo(RoomReleaseRequested),
                        noCompensation => noCompensation
                            .Then(context =>
                            {
                                _logger.LogInformation(
                                    "No compensation needed for BookingId: {BookingId} - no room hold reference found",
                                    context.Saga.BookingId);
                            })
                            .TransitionTo(Failed) // Don't finalize - keep for status queries
                    )
            );

            // Room release confirmation (compensation step)
            During(RoomReleaseRequested,
                When(RoomReleased)
                    .Then(context =>
                    {
                        context.Saga.UpdatedAt = DateTime.UtcNow;

                        _logger.LogInformation(
                            "Room release compensation completed for BookingId: {BookingId}, HoldReference: {RoomHoldReference}",
                            context.Saga.BookingId, context.Saga.RoomHoldReference);
                    })
                    .TransitionTo(Failed) // Don't finalize - keep for status queries
            );

            // Add state transition logging for all states
            DuringAny(
                When(BookingRequested)
                    .Then(context => LogStateTransition(context, "Initial", "Submitted")),
                When(RoomHeld)
                    .Then(context => LogStateTransition(context, "RoomHoldRequested", "PaymentProcessing")),
                When(RoomHoldFailed)
                    .Then(context => LogStateTransition(context, "RoomHoldRequested", "Failed")),
                When(PaymentSucceeded)
                    .Then(context => LogStateTransition(context, "PaymentProcessing", "Confirmed")),
                When(PaymentFailed)
                    .Then(context => LogStateTransition(context, "PaymentProcessing", "RoomReleaseRequested/Failed")),
                When(RoomReleased)
                    .Then(context => LogStateTransition(context, "RoomReleaseRequested", "Failed"))
            );

            // Clean up the saga when it's completed
            SetCompletedWhenFinalized();
        }

        private void LogStateTransition(BehaviorContext<BookingState> context, string fromState, string toState)
        {
            var saga = context.Saga;
            var duration = DateTime.UtcNow - saga.CreatedAt;
            
            _logger.LogInformation(
                "Saga state transition for BookingId: {BookingId} - {FromState} → {ToState}, " +
                "Duration: {Duration}ms, CorrelationId: {CorrelationId}",
                saga.BookingId, fromState, toState, duration.TotalMilliseconds, saga.CorrelationId);
        }
    }
}