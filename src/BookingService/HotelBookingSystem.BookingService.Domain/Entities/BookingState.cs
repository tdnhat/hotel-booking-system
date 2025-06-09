using HotelBookingSystem.BookingService.Domain.Enums;
using MassTransit;

namespace HotelBookingSystem.BookingService.Domain.Entities
{
    public class BookingState : SagaStateMachineInstance
    {
        // Required by MassTransit
        public Guid CorrelationId { get; set; }
        // Current state of the saga
        public string CurrentState { get; set; } = string.Empty;
        
        // Booking details
        public Guid BookingId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid HotelId { get; set; }
        public string GuestEmail { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }

        // Saga tracking
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? FailureReason { get; set; }

        // References for compensation
        public string? RoomHoldReference { get; set; }
        public string? PaymentReference { get; set; }

        // Concurrency control
        public byte[] Version { get; set; } = Array.Empty<byte>();

        // Helper properties
        public BookingStatus Status
        {
            get => Enum.TryParse<BookingStatus>(CurrentState, out var status) ? status : BookingStatus.Submitted;
            set => CurrentState = value.ToString();
        }
    }
}