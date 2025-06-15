using HotelBookingSystem.RoomManagementService.Domain.Common.BaseTypes;
using HotelBookingSystem.RoomManagementService.Domain.Common.Guards;
using HotelBookingSystem.RoomManagementService.Domain.Enums;

namespace HotelBookingSystem.RoomManagementService.Domain.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public Currency Currency { get; private set; }

        private Money() { }

        public Money(decimal amount, Currency currency)
        {
            Amount = Guard.Against.Negative(amount, nameof(amount));
            Currency = currency;
        }
        
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return new object[] { Amount, Currency };
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
            return new Money(Guard.Against.Negative(result, nameof(result)), Currency);
        }

        public Money Multiply(decimal factor)
        {
            var result = Amount * factor;
            return new Money(Guard.Against.Negative(result, nameof(result)), Currency);
        }

        public bool IsGreaterThan(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot compare money with different currencies");

            return Amount > other.Amount;
        }

        public override string ToString() => $"{Amount:C} {Currency}";

        public static Money Zero(Currency currency) => new(0, currency);
    }
}
