using HotelBookingSystem.Domain.Core.Common.BaseTypes;

namespace HotelBookingSystem.InventoryService.Domain.ValueObjects;

public class RoomTypeId : ValueObject
{
    public Guid Value { get; }

    private RoomTypeId(Guid value)
    {
        Value = value;
    }

    public static RoomTypeId New() => new(Guid.NewGuid());
    
    public static RoomTypeId From(Guid value) => new(value);

    public static implicit operator Guid(RoomTypeId id) => id.Value;
    
    public static implicit operator RoomTypeId(Guid value) => From(value);

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
} 