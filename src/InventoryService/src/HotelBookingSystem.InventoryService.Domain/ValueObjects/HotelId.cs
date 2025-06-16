using HotelBookingSystem.Domain.Core.Common.BaseTypes;

namespace HotelBookingSystem.InventoryService.Domain.ValueObjects;

public class HotelId : ValueObject
{
    public Guid Value { get; }

    private HotelId(Guid value)
    {
        Value = value;
    }

    public static HotelId New() => new(Guid.NewGuid());
    
    public static HotelId From(Guid value) => new(value);

    public static implicit operator Guid(HotelId id) => id.Value;
    
    public static implicit operator HotelId(Guid value) => From(value);

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
} 