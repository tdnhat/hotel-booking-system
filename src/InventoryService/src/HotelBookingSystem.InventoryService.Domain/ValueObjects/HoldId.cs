using HotelBookingSystem.Domain.Core.Common.BaseTypes;

namespace HotelBookingSystem.InventoryService.Domain.ValueObjects;

public class HoldId : ValueObject
{
    public Guid Value { get; }

    private HoldId(Guid value)
    {
        Value = value;
    }

    public static HoldId New() => new(Guid.NewGuid());
    
    public static HoldId From(Guid value) => new(value);

    public static implicit operator Guid(HoldId id) => id.Value;
    
    public static implicit operator HoldId(Guid value) => From(value);

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
} 