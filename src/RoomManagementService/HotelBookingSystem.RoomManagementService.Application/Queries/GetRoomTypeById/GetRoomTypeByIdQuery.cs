using HotelBookingSystem.RoomManagementService.Application.DTOs;
using MediatR;

namespace HotelBookingSystem.RoomManagementService.Application.Queries.GetRoomTypeById
{
    public record GetRoomTypeByIdQuery(Guid Id) : IRequest<RoomTypeDto?>;
}