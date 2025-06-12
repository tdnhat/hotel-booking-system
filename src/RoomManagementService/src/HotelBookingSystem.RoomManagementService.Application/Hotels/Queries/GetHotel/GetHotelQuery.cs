using HotelBookingSystem.RoomManagementService.Application.Common.Models;
using HotelBookingSystem.RoomManagementService.Application.DTOs;
using MediatR;

namespace HotelBookingSystem.RoomManagementService.Application.Hotels.Queries.GetHotel
{
    public record GetHotelQuery(Guid HotelId) : IRequest<Result<HotelDto>>;
}