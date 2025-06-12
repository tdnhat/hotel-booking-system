using HotelBookingSystem.RoomManagementService.Application.Common.Models;
using HotelBookingSystem.RoomManagementService.Application.DTOs;
using HotelBookingSystem.RoomManagementService.Domain.Enums;
using MediatR;

namespace HotelBookingSystem.RoomManagementService.Application.RoomTypes.Commands.AddRoomType
{
    public record AddRoomTypeCommand : IRequest<Result<RoomTypeDto>>
{
    public Guid HotelId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int MaxOccupancy { get; init; }
    public decimal BasePrice { get; init; }
    public Currency Currency { get; init; } = Currency.USD;
    public BedType PrimaryBedType { get; init; }
    public int PrimaryBedCount { get; init; }
    public BedType SecondaryBedType { get; init; } = BedType.None;
    public int SecondaryBedCount { get; init; }
    public List<string> Features { get; init; } = new();
}
}