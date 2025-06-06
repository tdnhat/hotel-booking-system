using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;

namespace HotelBookingSystem.RoomManagementService.Domain.Entities
{
    public class RoomType
    {
        public Guid Id { get; private set; }
        public Guid HotelId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal PricePerNight { get; private set; }
        public int MaxGuests { get; private set; }
        public int TotalRooms { get; private set; }

        // Navigation properties
        private readonly List<Room> _rooms = new();
        public IReadOnlyList<Room> Rooms => _rooms.AsReadOnly();

        // For EF Core
        private RoomType() { }

        public RoomType(Guid id, Guid hotelId, string name, string description, decimal pricePerNight, int maxGuests, int totalRooms)
        {
            Id = id;
            HotelId = hotelId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            PricePerNight = pricePerNight >= 0 ? pricePerNight : throw new ArgumentException("Price per night cannot be negative", nameof(pricePerNight));
            MaxGuests = maxGuests > 0 ? maxGuests : throw new ArgumentException("Max guests must be greater than 0", nameof(maxGuests));
            TotalRooms = totalRooms > 0 ? totalRooms : throw new ArgumentException("Total rooms must be greater than 0", nameof(totalRooms));
        }

        public bool IsAvailable(DateRange dateRange)
        {
            // Logic to check availability based on the date range
            return _rooms.Any(room => room.IsAvailable(dateRange));
        }

        public int GetAvailableRoomCount(DateRange dateRange)
        {
            // Count available rooms for the given date range
            return _rooms.Count(room => room.IsAvailable(dateRange));
        }

        public decimal CalculateTotalPrice(DateRange dateRange, DiscountPolicy? discountPolicy = null)
        {
            // Logic to calculate total price based on the date range and optional discount policy
            decimal totalPrice = CalculateBasePrice(dateRange);

            if (discountPolicy != null)
            {
                decimal discount = discountPolicy.ApplyDiscount(totalPrice);
                totalPrice -= discount;
            }

            return Math.Max(0, totalPrice); // Ensure price is never negative
        }

        public Money CalculateTotalPriceAsMoney(DateRange dateRange, DiscountPolicy? discountPolicy = null, string currency = "USD")
        {
            decimal price = CalculateTotalPrice(dateRange, discountPolicy);
            return new Money(price, currency);
        }

        public void UpdatePricing(decimal newPricePerNight)
        {
            if (newPricePerNight < 0)
                throw new ArgumentException("Price per night cannot be negative", nameof(newPricePerNight));
            
            PricePerNight = newPricePerNight;
        }

        public void UpdateDescription(string newDescription)
        {
            Description = newDescription ?? string.Empty;
        }

        private decimal CalculateBasePrice(DateRange dateRange)
        {
            // Logic to calculate base price based on the date range
            int nights = dateRange.NumberOfNights;
            return nights * PricePerNight;
        }
    }
}
