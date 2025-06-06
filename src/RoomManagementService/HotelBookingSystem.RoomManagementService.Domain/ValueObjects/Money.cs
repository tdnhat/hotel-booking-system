namespace HotelBookingSystem.RoomManagementService.Domain.ValueObjects
{
    public class Money : IEquatable<Money>
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        public Money(decimal amount, string currency = "USD")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"Cannot add different currencies: {Currency} and {other.Currency}");
            
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"Cannot subtract different currencies: {Currency} and {other.Currency}");
            
            return new Money(Amount - other.Amount, Currency);
        }

        public Money Multiply(decimal factor)
        {
            return new Money(Amount * factor, Currency);
        }

        public override bool Equals(object? obj) => 
            obj is Money other && Equals(other);

        public bool Equals(Money? other) =>
            other is not null && Amount == other.Amount && Currency == other.Currency;

        public override int GetHashCode() => 
            HashCode.Combine(Amount, Currency);

        public override string ToString() => 
            $"{Amount:C} {Currency}";

        public static bool operator ==(Money? left, Money? right) => 
            EqualityComparer<Money>.Default.Equals(left, right);

        public static bool operator !=(Money? left, Money? right) => 
            !(left == right);

        public static Money operator +(Money left, Money right) => 
            left.Add(right);

        public static Money operator -(Money left, Money right) => 
            left.Subtract(right);

        public static Money operator *(Money left, decimal right) => 
            left.Multiply(right);
    }
}
