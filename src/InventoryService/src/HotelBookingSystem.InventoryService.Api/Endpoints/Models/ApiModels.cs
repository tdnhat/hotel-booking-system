namespace HotelBookingSystem.InventoryService.Api.Endpoints.Models;

public record CheckAvailabilityResponse
{
    public bool IsAvailable { get; init; }
    public int AvailableRooms { get; init; }
    public decimal PricePerNight { get; init; }
    public string Currency { get; init; } = string.Empty;
    public decimal TotalPrice { get; init; }
    public string? Message { get; init; }
}

public record CreateRoomHoldResponse
{
    public Guid Id { get; init; }
    public Guid BookingId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public string? HoldReference { get; init; }
}

public record GetRoomHoldResponse
{
    public Guid Id { get; init; }
    public Guid BookingId { get; init; }
    public Guid HotelId { get; init; }
    public Guid RoomTypeId { get; init; }
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }
    public int RoomCount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public string? HoldReference { get; init; }
} 