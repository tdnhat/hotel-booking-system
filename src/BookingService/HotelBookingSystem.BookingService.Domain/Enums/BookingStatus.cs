namespace HotelBookingSystem.BookingService.Domain.Enums
{
    public enum BookingStatus
    {
        // Initial state
        Submitted,

        // Room hold process
        RoomHoldRequested,
        RoomHeld,
        RoomHoldFailed,

        // Payment process
        PaymentProcessing,
        PaymentSucceeded,
        PaymentFailed,

        // Room release process
        RoomReleaseRequested,
        RoomReleased,

        // Final states
        Confirmed,
        Failed,
        Cancelled,
    }
}