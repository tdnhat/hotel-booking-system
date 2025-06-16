using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.InventoryService.Domain.Enums;

namespace HotelBookingSystem.InventoryService.Domain.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    private Money() { }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
        
        Amount = amount;
        Currency = currency;
    }
    
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException("Cannot add money with different currencies");
        }
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");

        var result = Amount - other.Amount;
        if (result < 0)
            throw new ArgumentException("Result cannot be negative");
        
        return new Money(result, Currency);
    }

    public Money Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("Factor cannot be negative");
        
        var result = Amount * factor;
        return new Money(result, Currency);
    }

    public bool IsGreaterThan(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");

        return Amount > other.Amount;
    }

    public bool IsGreaterThanOrEqual(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");

        return Amount >= other.Amount;
    }

    public override string ToString() => $"{Amount:C} {Currency}";

    public static Money Zero(Currency currency) => new(0, currency);
} 