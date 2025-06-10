using HotelBookingSystem.BookingService.Domain.Enums;

namespace HotelBookingSystem.BookingService.Application.DTOs
{
    public class BookingResponse
    {
        public Guid BookingId { get; set; }
        public Guid CorrelationId { get; set; }
        public BookingStatus Status { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid HotelId { get; set; }
        public string GuestEmail { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? FailureReason { get; set; }
        public string? RoomHoldReference { get; set; }
        public string? PaymentReference { get; set; }
    }
}