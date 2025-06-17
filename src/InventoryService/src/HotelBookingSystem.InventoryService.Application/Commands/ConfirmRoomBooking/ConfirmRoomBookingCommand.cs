using HotelBookingSystem.InventoryService.Application.Common.Models;
using MediatR;

namespace HotelBookingSystem.InventoryService.Application.Commands.ConfirmRoomBooking;

public record ConfirmRoomBookingCommand : IRequest<Result>
{
    public Guid BookingId { get; init; }
    public Guid HoldId { get; init; }
    public Guid PaymentId { get; init; }
} 