using HotelBookingSystem.RoomManagementService.Application.DTOs;
using MediatR;

namespace HotelBookingSystem.RoomManagementService.Application.Queries.GetRoomTypes
{
    public class GetRoomTypesQuery : IRequest<IEnumerable<RoomTypeDto>>
    {
        public Guid? HotelId { get; set; }
    }
}
