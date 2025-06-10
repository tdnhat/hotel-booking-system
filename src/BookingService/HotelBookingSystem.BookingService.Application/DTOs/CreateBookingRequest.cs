namespace HotelBookingSystem.BookingService.Application.DTOs
{
    public class CreateBookingRequest
    {
        public Guid RoomTypeId { get; set; }
        public Guid HotelId { get; set; }
        public string GuestEmail { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }

        public bool IsValidDateRange()
        {
            return CheckOutDate > CheckInDate && CheckInDate >= DateTime.Today;
        }
    }
}