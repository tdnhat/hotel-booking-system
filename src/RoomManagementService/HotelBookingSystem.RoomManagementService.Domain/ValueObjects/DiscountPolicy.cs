namespace HotelBookingSystem.RoomManagementService.Domain.ValueObjects
{
    public abstract class DiscountPolicy
    {
        public abstract decimal ApplyDiscount(decimal originalPrice);
    }

    public class PercentageDiscountPolicy : DiscountPolicy
    {
        private readonly decimal _percentage;

        public PercentageDiscountPolicy(decimal percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Percentage must be between 0 and 100", nameof(percentage));
            
            _percentage = percentage;
        }

        public override decimal ApplyDiscount(decimal originalPrice)
        {
            return originalPrice * (_percentage / 100);
        }
    }

    public class FixedAmountDiscountPolicy : DiscountPolicy
    {
        private readonly decimal _amount;

        public FixedAmountDiscountPolicy(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Discount amount cannot be negative", nameof(amount));
            
            _amount = amount;
        }

        public override decimal ApplyDiscount(decimal originalPrice)
        {
            return Math.Min(_amount, originalPrice);
        }
    }

    public class NoDiscountPolicy : DiscountPolicy
    {
        public override decimal ApplyDiscount(decimal originalPrice)
        {
            return 0;
        }
    }
}
