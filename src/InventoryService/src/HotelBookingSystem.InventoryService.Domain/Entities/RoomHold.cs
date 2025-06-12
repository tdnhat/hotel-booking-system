namespace HotelBookingSystem.InventoryService.Domain.Entities
{
    public class RoomHold
    {
        public Guid Id { get; set; }
        public string HoldReference { get; set; } = string.Empty;
        public Guid BookingId { get; set; }
        public Guid RoomTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsReleased { get; set; } = false;
        public DateTime? ReleasedAt { get; set; }
    }
} 