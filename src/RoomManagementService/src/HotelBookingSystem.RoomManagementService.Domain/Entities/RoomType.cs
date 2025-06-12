using HotelBookingSystem.RoomManagementService.Domain.Common.BaseTypes;
using HotelBookingSystem.RoomManagementService.Domain.Common.Guards;
using HotelBookingSystem.RoomManagementService.Domain.Enums;
using HotelBookingSystem.RoomManagementService.Domain.Events;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;

namespace HotelBookingSystem.RoomManagementService.Domain.Entities
{
    public class RoomType : Entity<RoomTypeId>
    {
        public HotelId HotelId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int MaxOccupancy { get; private set; }
        public Money BasePrice { get; private set; }
        public BedConfiguration BedConfiguration { get; private set; }
        public RoomFeatures Features { get; private set; }
        public RoomTypeStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private RoomType() { } // EF Core constructor

        internal RoomType(
            RoomTypeId id,
            HotelId hotelId,
            string name,
            string description,
            int maxOccupancy,
            Money basePrice,
            BedConfiguration bedConfiguration)
        {
            Id = id;
            HotelId = hotelId;
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Description = description ?? string.Empty;
            MaxOccupancy = Guard.Against.NegativeOrZero(maxOccupancy, nameof(maxOccupancy));
            BasePrice = Guard.Against.Null(basePrice, nameof(basePrice));
            BedConfiguration = Guard.Against.Null(bedConfiguration, nameof(bedConfiguration));
            Features = RoomFeatures.Empty();
            Status = RoomTypeStatus.Available;
            CreatedAt = DateTime.UtcNow;

            ValidateOccupancyAgainstBedConfiguration();
        }

        public void UpdateDetails(string name, string description, int maxOccupancy)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Description = description ?? string.Empty;
            MaxOccupancy = Guard.Against.NegativeOrZero(maxOccupancy, nameof(maxOccupancy));
            UpdatedAt = DateTime.UtcNow;

            ValidateOccupancyAgainstBedConfiguration();

            RaiseDomainEvent(new RoomTypeUpdatedDomainEvent(
                HotelId, Id, Name, MaxOccupancy, BasePrice.Amount));
        }

        public void UpdatePrice(Money newBasePrice)
        {
            BasePrice = Guard.Against.Null(newBasePrice, nameof(newBasePrice));
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new RoomTypeUpdatedDomainEvent(
                HotelId, Id, Name, MaxOccupancy, BasePrice.Amount));
        }

        public void UpdateBedConfiguration(BedConfiguration newBedConfiguration)
        {
            BedConfiguration = Guard.Against.Null(newBedConfiguration, nameof(newBedConfiguration));
            UpdatedAt = DateTime.UtcNow;

            ValidateOccupancyAgainstBedConfiguration();
        }

        public void AddFeature(string featureName)
        {
            Features = Features.AddFeature(featureName);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveFeature(string featureName)
        {
            Features = Features.RemoveFeature(featureName);
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetFeatures(RoomFeatures newFeatures)
        {
            Features = Guard.Against.Null(newFeatures, nameof(newFeatures));
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            Status = RoomTypeStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            Status = RoomTypeStatus.Unavailable;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Discontinue()
        {
            Status = RoomTypeStatus.Discontinued;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsAvailable => Status == RoomTypeStatus.Available;

        public bool CanAccommodate(int guestCount) => guestCount <= MaxOccupancy && guestCount > 0;

        private void ValidateOccupancyAgainstBedConfiguration()
        {
            var bedCapacity = BedConfiguration.CalculateMaxCapacity();
            if (MaxOccupancy > bedCapacity)
            {
                throw new InvalidOperationException(
                    $"Max occupancy ({MaxOccupancy}) cannot exceed bed configuration capacity ({bedCapacity})");
            }
        }
    }
}
