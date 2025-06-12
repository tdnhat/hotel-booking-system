namespace HotelBookingSystem.RoomManagementService.Domain.ValueObjects
{
    public record HotelId(Guid Value)
    {
        public static HotelId New() => new(Guid.NewGuid());
        public static HotelId From(Guid value) => new(value);
        public static HotelId From(string value) => new(Guid.Parse(value));
        public override string ToString() => Value.ToString();
        public static implicit operator Guid(HotelId hotelId) => hotelId.Value;
        public static implicit operator HotelId(Guid value) => new(value);
    }
}
