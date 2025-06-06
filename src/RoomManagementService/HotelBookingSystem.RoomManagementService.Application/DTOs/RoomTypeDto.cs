namespace HotelBookingSystem.RoomManagementService.Application.DTOs
{
    public class RoomTypeDto
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
    }
}
