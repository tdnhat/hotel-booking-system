namespace HotelBookingSystem.RoomManagementService.Domain.ValueObjects
{
    public class RoomTypeId(Guid value)
    {
        public Guid Value { get; } = value;

        public static RoomTypeId New() => new(Guid.NewGuid());

        public static RoomTypeId From(Guid value) => new(value);

        public static RoomTypeId From(string value) => new(Guid.Parse(value));

        public override string ToString() => Value.ToString();

        public static implicit operator Guid(RoomTypeId roomTypeId) => roomTypeId.Value;

        public static implicit operator RoomTypeId(Guid value) => new(value);
    }
}
