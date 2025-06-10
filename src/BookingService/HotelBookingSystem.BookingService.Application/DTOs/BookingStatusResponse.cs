using HotelBookingSystem.BookingService.Domain.Enums;

namespace HotelBookingSystem.BookingService.Application.DTOs
{
    public class BookingStatusResponse
    {
        public Guid BookingId { get; set; }
        public BookingStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public string? FailureReason { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsSuccess { get; set; }

        // For UI display
        public string StatusMessage { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }

        // Booking Details
        public BookingDetailsDto BookingDetails { get; set; } = new();
    }

    public class BookingDetailsDto
    {
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public string GuestEmail { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfNights { get; set; }
        public decimal PricePerNight { get; set; }
    }
}