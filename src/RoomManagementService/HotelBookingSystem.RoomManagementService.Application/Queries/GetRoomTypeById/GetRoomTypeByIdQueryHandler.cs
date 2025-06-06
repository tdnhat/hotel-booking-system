using HotelBookingSystem.RoomManagementService.Application.DTOs;
using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using MediatR;

namespace HotelBookingSystem.RoomManagementService.Application.Queries.GetRoomTypeById
{
    public class GetRoomTypeByIdQueryHandler : IRequestHandler<GetRoomTypeByIdQuery, RoomTypeDto?>
    {
        private readonly IRoomTypeRepository _roomTypeRepository;

        public GetRoomTypeByIdQueryHandler(IRoomTypeRepository roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }

        public async Task<RoomTypeDto?> Handle(GetRoomTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(request.Id, cancellationToken);
            return roomType is null ? null : new RoomTypeDto
            {
                Id = roomType.Id,
                HotelId = roomType.HotelId,
                Name = roomType.Name,
                Description = roomType.Description,
                PricePerNight = roomType.PricePerNight,
                MaxGuests = roomType.MaxGuests,
                TotalRooms = roomType.TotalRooms,
                AvailableRooms = roomType.Rooms.Count(r => r.Status == Domain.Enums.RoomStatus.Available)
            };
        }
    }
}