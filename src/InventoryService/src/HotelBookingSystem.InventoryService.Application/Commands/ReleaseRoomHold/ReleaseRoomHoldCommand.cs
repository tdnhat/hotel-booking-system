using HotelBookingSystem.InventoryService.Application.Common.Models;
using MediatR;

namespace HotelBookingSystem.InventoryService.Application.Commands.ReleaseRoomHold;

public record ReleaseRoomHoldCommand : IRequest<Result>
{
    public Guid BookingId { get; init; }
    public Guid HoldId { get; init; }
    public string Reason { get; init; } = string.Empty;
} 