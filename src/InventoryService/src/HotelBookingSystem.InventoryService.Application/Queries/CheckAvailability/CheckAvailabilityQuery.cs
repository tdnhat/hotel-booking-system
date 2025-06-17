using HotelBookingSystem.InventoryService.Application.Common.Models;
using HotelBookingSystem.InventoryService.Application.DTOs;
using MediatR;

namespace HotelBookingSystem.InventoryService.Application.Queries.CheckAvailability;

public record CheckAvailabilityQuery : IRequest<Result<DateRangeAvailabilityDto>>
{
    public Guid HotelId { get; init; }
    public Guid RoomTypeId { get; init; }
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }
    public int RequestedRooms { get; init; }
} 