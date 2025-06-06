using HotelBookingSystem.RoomManagementService.Domain.Enums;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;

namespace HotelBookingSystem.RoomManagementService.Domain.Entities
{
    public class Room
    {
        public Guid Id { get; private set; }
        public Guid RoomTypeId { get; private set; }
        public string RoomNumber { get; private set; } = string.Empty;
        public RoomStatus Status { get; private set; } = RoomStatus.Available;

        // For EF Core
        private Room() { }

        public Room(Guid id, Guid roomTypeId, string roomNumber)
        {
            Id = id;
            RoomTypeId = roomTypeId;
            RoomNumber = roomNumber ?? throw new ArgumentNullException(nameof(roomNumber));
            Status = RoomStatus.Available;
        }

        public bool IsAvailable(DateRange dateRange)
        {
            // Logic to check availability based on the date range
            // This is a placeholder implementation; actual logic would involve checking existing bookings
            return Status == RoomStatus.Available;
        }

        public void MarkAsReserved()
        {
            if (Status != RoomStatus.Available)
                throw new InvalidOperationException($"Cannot reserve room {RoomNumber}. Current status: {Status}");
            
            Status = RoomStatus.Reserved;
        }

        public void Release()
        {
            if (Status != RoomStatus.Reserved)
                throw new InvalidOperationException($"Cannot release room {RoomNumber}. Current status: {Status}");
            
            Status = RoomStatus.Available;
        }

        public void MarkAsOutOfService()
        {
            Status = RoomStatus.OutOfService;
        }

        public void MarkAsAvailable()
        {
            if (Status == RoomStatus.OutOfService)
                throw new InvalidOperationException($"Room {RoomNumber} is out of service and cannot be marked as available");
            
            Status = RoomStatus.Available;
        }
    }
}
