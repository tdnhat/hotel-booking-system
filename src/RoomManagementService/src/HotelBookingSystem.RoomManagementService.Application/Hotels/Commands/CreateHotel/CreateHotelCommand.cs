using HotelBookingSystem.RoomManagementService.Application.Common.Models;
using HotelBookingSystem.RoomManagementService.Application.DTOs;
using HotelBookingSystem.RoomManagementService.Domain.Enums;
using MediatR;

namespace HotelBookingSystem.RoomManagementService.Application.Hotels.Commands.CreateHotel
{
    public record CreateHotelCommand : IRequest<Result<HotelDto>>
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public StarRating StarRating { get; init; }
        public string? ContactEmail { get; init; }
        public string? ContactPhone { get; init; }
        public CreateAddressDto Address { get; init; } = new();
    }

    public record CreateAddressDto
    {
        public string Street { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
    }
}