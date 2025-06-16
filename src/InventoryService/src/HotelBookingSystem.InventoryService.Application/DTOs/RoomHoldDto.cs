namespace HotelBookingSystem.InventoryService.Application.DTOs;

public class RoomHoldDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int RoomCount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string HoldReference { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public string? ReleaseReason { get; set; }
    public TimeSpan RemainingTime { get; set; }
    public bool IsExpired { get; set; }
    public bool IsActive { get; set; }
}

public class CreateRoomHoldDto
{
    public Guid BookingId { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int RoomCount { get; set; }
    public TimeSpan HoldDuration { get; set; } = TimeSpan.FromMinutes(15);
} 