using HotelBookingSystem.RoomManagementService.Application.Common.Exceptions;
using HotelBookingSystem.RoomManagementService.Application.Common.Models;
using HotelBookingSystem.RoomManagementService.Application.DTOs;
using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;
using MediatR;

namespace HotelBookingSystem.RoomManagementService.Application.RoomTypes.Commands.AddRoomType
{
    public class AddRoomTypeCommandHandler : IRequestHandler<AddRoomTypeCommand, Result<RoomTypeDto>>
    {
        private readonly IHotelRepository _hotelRepository;

        public AddRoomTypeCommandHandler(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task<Result<RoomTypeDto>> Handle(AddRoomTypeCommand request, CancellationToken cancellationToken)
        {
            var hotelId = HotelId.From(request.HotelId);
            var hotel = await _hotelRepository.GetByIdWithRoomTypesAsync(hotelId, cancellationToken);

            if (hotel is null)
            {
                throw new NotFoundException(nameof(hotel), request.HotelId);
            }

            // Create value objects
            var money = new Money(request.BasePrice, request.Currency);
            var bedConfiguration = new BedConfiguration(
                request.PrimaryBedType,
                request.PrimaryBedCount,
                request.SecondaryBedType,
                request.SecondaryBedCount);

            // Add room type to hotel (domain logic)
            var roomTypeId = hotel.AddRoomType(
                request.Name,
                request.Description,
                request.MaxOccupancy,
                money,
                bedConfiguration);

            // Add features if provided
            var roomType = hotel.GetRoomType(roomTypeId);
            var features = RoomFeatures.Create(request.Features);
            roomType.SetFeatures(features);

            // Save changes
            await _hotelRepository.UpdateAsync(hotel, cancellationToken);

            // Map to DTO
            var roomTypeDto = new RoomTypeDto
            {
                Id = roomType.Id.Value,
                Name = roomType.Name,
                Description = roomType.Description,
                MaxOccupancy = roomType.MaxOccupancy,
                BasePrice = roomType.BasePrice.Amount,
                Currency = roomType.BasePrice.Currency,
                Status = roomType.Status,
                BedConfiguration = new BedConfigurationDto
                {
                    PrimaryBedType = roomType.BedConfiguration.PrimaryBedType,
                    PrimaryBedCount = roomType.BedConfiguration.PrimaryBedCount,
                    SecondaryBedType = roomType.BedConfiguration.SecondaryBedType,
                    SecondaryBedCount = roomType.BedConfiguration.SecondaryBedCount,
                    Description = roomType.BedConfiguration.GetDescription()
                },
                Features = roomType.Features.Features.ToList()
            };

            return Result.Success(roomTypeDto);
        }
    }
}