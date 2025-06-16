using HotelBookingSystem.Domain.Core.Common.BaseTypes;

namespace HotelBookingSystem.InventoryService.Domain.ValueObjects;

public class InventoryId : ValueObject
{
    public Guid Value { get; }

    private InventoryId(Guid value)
    {
        Value = value;
    }

    public static InventoryId New() => new(Guid.NewGuid());
    
    public static InventoryId From(Guid value) => new(value);

    public static implicit operator Guid(InventoryId id) => id.Value;
    
    public static implicit operator InventoryId(Guid value) => From(value);

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
} 