using HotelBookingSystem.InventoryService.Application.Common.Models;
using HotelBookingSystem.InventoryService.Application.DTOs;
using MediatR;

namespace HotelBookingSystem.InventoryService.Application.Queries.GetRoomHold;

public record GetRoomHoldQuery : IRequest<Result<RoomHoldDto>>
{
    public Guid HoldId { get; init; }
    public Guid? BookingId { get; init; }
} 