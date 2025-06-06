using HotelBookingSystem.RoomManagementService.Application.DTOs;
using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using MediatR;

namespace HotelBookingSystem.RoomManagementService.Application.Queries.GetRoomTypes
{
    public class GetRoomTypesQueryHandler : IRequestHandler<GetRoomTypesQuery, IEnumerable<RoomTypeDto>>
    {
        private readonly IRoomTypeRepository _roomTypeRepository;

        public GetRoomTypesQueryHandler(IRoomTypeRepository roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }

        public async Task<IEnumerable<RoomTypeDto>> Handle(GetRoomTypesQuery request, CancellationToken cancellationToken)
        {
            var roomTypes = request.HotelId.HasValue
                ? await _roomTypeRepository.GetByHotelIdAsync(request.HotelId.Value, cancellationToken)
                : await _roomTypeRepository.GetAllAsync(cancellationToken);

            return roomTypes is null ? Enumerable.Empty<RoomTypeDto>() : roomTypes.Select(rt => new RoomTypeDto
            {
                Id = rt.Id,
                HotelId = rt.HotelId,
                Name = rt.Name,
                Description = rt.Description,
                PricePerNight = rt.PricePerNight,
                MaxGuests = rt.MaxGuests,
                TotalRooms = rt.TotalRooms,
                AvailableRooms = rt.Rooms.Count(r => r.Status == Domain.Enums.RoomStatus.Available)
            });
        }
    }
}
