using HotelBookingSystem.InventoryService.Application.Common.Models;
using HotelBookingSystem.InventoryService.Application.DTOs;
using MediatR;

namespace HotelBookingSystem.InventoryService.Application.Commands.HoldRoom;

public record HoldRoomCommand : IRequest<Result<RoomHoldDto>>
{
    public Guid BookingId { get; init; }
    public Guid HotelId { get; init; }
    public Guid RoomTypeId { get; init; }
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }
    public int RoomCount { get; init; }
    public TimeSpan HoldDuration { get; init; } = TimeSpan.FromMinutes(15);
} 