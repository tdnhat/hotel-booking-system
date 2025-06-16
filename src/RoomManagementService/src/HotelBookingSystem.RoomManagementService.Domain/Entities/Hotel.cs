using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.Domain.Core.Common.Exceptions;
using HotelBookingSystem.Domain.Core.Common.Guards;
using HotelBookingSystem.RoomManagementService.Domain.Enums;
using HotelBookingSystem.RoomManagementService.Domain.Events;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;

namespace HotelBookingSystem.RoomManagementService.Domain.Entities
{
    public class Hotel : Entity<HotelId>
    {
        private readonly List<RoomType> _roomTypes = new();

        public string Name { get; private set; }
        public Address Address { get; private set; }
        public string Description { get; private set; }
        public HotelStatus Status { get; private set; }
        public StarRating StarRating { get; private set; }
        public string? ContactEmail { get; private set; }
        public string? ContactPhone { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public IReadOnlyCollection<RoomType> RoomTypes => _roomTypes.AsReadOnly();

        private Hotel() { } // EF Core constructor

        public Hotel(
            HotelId id,
            string name,
            Address address,
            string description,
            StarRating starRating,
            string? contactEmail = null,
            string? contactPhone = null)
        {
            Id = id;
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Address = Guard.Against.Null(address, nameof(address));
            Description = description ?? string.Empty;
            StarRating = starRating;
            ContactEmail = contactEmail;
            ContactPhone = contactPhone;
            Status = HotelStatus.Active;
            CreatedAt = DateTime.UtcNow;

            ValidateContactInformation();

            RaiseDomainEvent(new HotelCreatedDomainEvent(Id, Name, Address.GetFullAddress()));
        }

        public void UpdateBasicDetails(string name, string description, StarRating starRating)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Description = description ?? string.Empty;
            StarRating = starRating;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new HotelUpdatedDomainEvent(Id, Name, Address.GetFullAddress()));
        }

        public void UpdateAddress(Address newAddress)
        {
            Address = Guard.Against.Null(newAddress, nameof(newAddress));
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new HotelUpdatedDomainEvent(Id, Name, Address.GetFullAddress()));
        }

        public void UpdateContactInformation(string? contactEmail, string? contactPhone)
        {
            ContactEmail = contactEmail;
            ContactPhone = contactPhone;
            UpdatedAt = DateTime.UtcNow;

            ValidateContactInformation();
        }

        public RoomTypeId AddRoomType(
            string name,
            string description,
            int maxOccupancy,
            Money basePrice,
            BedConfiguration bedConfiguration)
        {
            if (Status != HotelStatus.Active)
            {
                throw new BusinessRuleViolationException("Cannot add room types to inactive hotels");
            }

            var roomTypeId = RoomTypeId.New();
            var roomType = new RoomType(roomTypeId, Id, name, description, maxOccupancy, basePrice, bedConfiguration);

            if (HasRoomTypeWithName(name))
            {
                throw new BusinessRuleViolationException($"Room type with name '{name}' already exists in this hotel");
            }

            _roomTypes.Add(roomType);

            RaiseDomainEvent(new RoomTypeAddedDomainEvent(
                Id, roomTypeId, name, maxOccupancy, basePrice.Amount));

            return roomTypeId;
        }

        public void RemoveRoomType(RoomTypeId roomTypeId)
        {
            var roomType = GetRoomType(roomTypeId);
            roomType.Discontinue();
            UpdatedAt = DateTime.UtcNow;
        }

        public RoomType GetRoomType(RoomTypeId roomTypeId)
        {
            var roomType = _roomTypes.FirstOrDefault(rt => rt.Id == roomTypeId);
            if (roomType == null)
            {
                throw new BusinessRuleViolationException($"Room type with ID '{roomTypeId}' not found in hotel '{Name}'");
            }
            return roomType;
        }

        public IEnumerable<RoomType> GetAvailableRoomTypes()
        {
            return _roomTypes.Where(rt => rt.IsAvailable);
        }

        public void Activate()
        {
            if (Status == HotelStatus.Active)
                return;

            Status = HotelStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            Status = HotelStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;

            // Deactivate all room types when hotel is deactivated
            foreach (var roomType in _roomTypes.Where(rt => rt.IsAvailable))
            {
                roomType.Deactivate();
            }

            RaiseDomainEvent(new HotelDeactivatedDomainEvent(Id));
        }

        public void SetUnderMaintenance()
        {
            Status = HotelStatus.UnderMaintenance;
            UpdatedAt = DateTime.UtcNow;

            foreach (var roomType in _roomTypes.Where(rt => rt.IsAvailable))
            {
                roomType.Deactivate();
            }
        }

        public void SetPendingApproval()
        {
            Status = HotelStatus.PendingApproval;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsOperational => Status == HotelStatus.Active;

        public bool HasAvailableRoomTypes => _roomTypes.Any(rt => rt.IsAvailable);

        public int TotalRoomTypeCount => _roomTypes.Count;

        public int ActiveRoomTypeCount => _roomTypes.Count(rt => rt.IsAvailable);

        private bool HasRoomTypeWithName(string name)
        {
            return _roomTypes.Any(rt => rt.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private void ValidateContactInformation()
        {
            if (string.IsNullOrWhiteSpace(ContactEmail) && string.IsNullOrWhiteSpace(ContactPhone))
            {
                throw new BusinessRuleViolationException("Hotel must have at least one contact method (email or phone)");
            }

            if (!string.IsNullOrWhiteSpace(ContactEmail) && !IsValidEmail(ContactEmail))
            {
                throw new BusinessRuleViolationException("Contact email format is invalid");
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
