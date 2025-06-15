using HotelBookingSystem.RoomManagementService.Domain.Enums;

namespace HotelBookingSystem.RoomManagementService.Application.DTOs
{
    public record HotelDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public AddressDto Address { get; init; } = new();
        public string Description { get; init; } = string.Empty;
        public HotelStatus Status { get; init; }
        public StarRating StarRating { get; init; }
        public string? ContactEmail { get; init; }
        public string? ContactPhone { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public List<RoomTypeDto> RoomTypes { get; init; } = new();
    }
}