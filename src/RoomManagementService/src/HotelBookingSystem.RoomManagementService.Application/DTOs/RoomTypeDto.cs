using HotelBookingSystem.RoomManagementService.Domain.Enums;

namespace HotelBookingSystem.RoomManagementService.Application.DTOs
{
    public record RoomTypeDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int MaxOccupancy { get; init; }
        public decimal BasePrice { get; init; }
        public Currency Currency { get; init; }
        public BedConfigurationDto BedConfiguration { get; init; } = new();
        public List<string> Features { get; init; } = new();
        public RoomTypeStatus Status { get; init; }
    }

    public record BedConfigurationDto
    {
        public BedType PrimaryBedType { get; init; }
        public int PrimaryBedCount { get; init; }
        public BedType SecondaryBedType { get; init; }
        public int SecondaryBedCount { get; init; }
        public string Description { get; init; } = string.Empty;
    }
}