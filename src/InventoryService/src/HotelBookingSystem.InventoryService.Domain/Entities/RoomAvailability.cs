using HotelBookingSystem.InventoryService.Domain.Enums;

namespace HotelBookingSystem.InventoryService.Domain.Entities
{
    public class RoomAvailability
    {
        public Guid Id { get; set; }
        public Guid RoomTypeId { get; set; }
        public DateTime Date { get; set; }
        public AvailabilityStatus Status { get; set; } = AvailabilityStatus.Available;
        public Guid? BookingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
} 