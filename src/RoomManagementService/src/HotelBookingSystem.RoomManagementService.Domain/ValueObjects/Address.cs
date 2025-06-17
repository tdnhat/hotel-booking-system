using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.Domain.Core.Common.Guards;

namespace HotelBookingSystem.RoomManagementService.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string PostalCode { get; private set; }

        private Address() { } // EF Core constructor

        public Address(string street, string city, string state, string country, string postalCode)
        {
            Street = Guard.Against.NullOrWhiteSpace(street, nameof(street));
            City = Guard.Against.NullOrWhiteSpace(city, nameof(city));
            State = Guard.Against.NullOrWhiteSpace(state, nameof(state));
            Country = Guard.Against.NullOrWhiteSpace(country, nameof(country));
            PostalCode = Guard.Against.NullOrWhiteSpace(postalCode, nameof(postalCode));
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return new object[] { Street, City, State, Country, PostalCode };
        }

        public string GetFullAddress() => $"{Street}, {City}, {State} {PostalCode}, {Country}";

        public string GetCityStateCountry() => $"{City}, {State}, {Country}";
    }
}
