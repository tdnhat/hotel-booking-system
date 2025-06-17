namespace HotelBookingSystem.InventoryService.Application.DTOs;

public class RoomInventoryDto
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int HeldRooms { get; set; }
    public int BookedRooms { get; set; }
    public decimal BasePrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}

public class RoomAvailabilityDto
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public int AvailableRooms { get; set; }
    public int TotalRooms { get; set; }
    public decimal CurrentPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}

public class DateRangeAvailabilityDto
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int RequestedRooms { get; set; }
    public bool IsAvailable { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<RoomAvailabilityDto> DailyAvailability { get; set; } = new();
} 