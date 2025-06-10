using HotelBookingSystem.BookingService.Domain.Entities;
using HotelBookingSystem.BookingService.Domain.Messages.Commands;
using HotelBookingSystem.BookingService.Domain.Messages.Events;
using MassTransit;

namespace HotelBookingSystem.BookingService.Infrastructure.Saga
{
    /// <summary>
    /// Saga State Machine that orchestrates the booking process
    /// Handles the complete workflow from booking request to confirmation or failure
    /// </summary>
    public class BookingSagaStateMachine : MassTransitStateMachine<BookingState>
    {
        // State declarations
        public State Submitted { get; private set; } = null!;
        public State RoomHoldRequested { get; private set; } = null!;
        public State RoomHeldState { get; private set; } = null!;
        public State PaymentProcessing { get; private set; } = null!;
        public State RoomReleaseRequested { get; private set; } = null!;
        public State Confirmed { get; private set; } = null!;
        public State Failed { get; private set; } = null!;

        // Event declarations
        public Event<BookingRequested> BookingRequested { get; private set; } = null!;
        public Event<RoomHeld> RoomHeld { get; private set; } = null!;
        public Event<RoomHoldFailed> RoomHoldFailed { get; private set; } = null!;
        public Event<PaymentSucceeded> PaymentSucceeded { get; private set; } = null!;
        public Event<PaymentFailed> PaymentFailed { get; private set; } = null!;
        public Event<RoomReleased> RoomReleased { get; private set; } = null!;

        public BookingSagaStateMachine()
        {
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
                    .TransitionTo(RoomHoldRequested) // Wait for room hold response
            );

            // Room hold responses
            During(RoomHoldRequested,
                When(RoomHeld)
                    .Then(context =>
                    {
                        context.Saga.RoomHoldReference = context.Message.RoomHoldReference;
                        context.Saga.UpdatedAt = DateTime.UtcNow;
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
                    .TransitionTo(PaymentProcessing),
                When(RoomHoldFailed)
                    .Then(context =>
                    {
                        context.Saga.FailureReason = $"Room hold failed: {context.Message.Reason}";
                        context.Saga.UpdatedAt = DateTime.UtcNow;
                    })
                    .TransitionTo(Failed) // Don't finalize - keep for status queries
            );

            // Payment processing responses
            During(PaymentProcessing,
                When(PaymentSucceeded)
                    .Then(context =>
                    {
                        context.Saga.PaymentReference = context.Message.PaymentReference;
                        context.Saga.UpdatedAt = DateTime.UtcNow;
                    })
                    .TransitionTo(Confirmed), // Success - keep for status queries
                When(PaymentFailed)
                    .Then(context =>
                    {
                        context.Saga.FailureReason = $"Payment failed: {context.Message.Reason}";
                        context.Saga.UpdatedAt = DateTime.UtcNow;
                    })
                    // Compensate by releasing the room if we have a hold reference
                    .IfElse(context => !string.IsNullOrEmpty(context.Saga.RoomHoldReference),
                        compensate => compensate
                            .PublishAsync(context => context.Init<ReleaseRoom>(new
                            {
                                BookingId = context.Saga.BookingId,
                                RoomHoldReference = context.Saga.RoomHoldReference,
                                ReleasedAt = DateTime.UtcNow
                            }))
                            .TransitionTo(RoomReleaseRequested),
                        noCompensation => noCompensation
                            .TransitionTo(Failed) // Don't finalize - keep for status queries
                    )
            );

            // Room release confirmation (compensation step)
            During(RoomReleaseRequested,
                When(RoomReleased)
                    .Then(context =>
                    {
                        context.Saga.UpdatedAt = DateTime.UtcNow;
                    })
                    .TransitionTo(Failed) // Don't finalize - keep for status queries
            );

            // Clean up the saga when it's completed
            SetCompletedWhenFinalized();
        }
    }
}