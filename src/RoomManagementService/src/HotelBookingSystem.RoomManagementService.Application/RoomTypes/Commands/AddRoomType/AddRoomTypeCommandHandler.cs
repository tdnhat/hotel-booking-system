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

            // Add features if provided, otherwise apply defaults based on room characteristics
            var roomType = hotel.GetRoomType(roomTypeId);
            
            RoomFeatures features;
            if (request.Features.Any())
            {
                // Use manually provided features
                features = RoomFeatures.Create(request.Features);
            }
            else
            {
                // Auto-assign features based on room characteristics
                features = DetermineDefaultFeatures(request.Name, request.BasePrice, bedConfiguration);
            }
            
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
                Features = roomType.Features?.Features.ToList() ?? new List<string>()
            };

            return Result.Success(roomTypeDto);
        }

        private static RoomFeatures DetermineDefaultFeatures(string roomName, decimal basePrice, BedConfiguration bedConfiguration)
        {
            var roomNameLower = roomName.ToLowerInvariant();
            
            // Luxury features for expensive rooms or suites
            if (basePrice >= 200 || roomNameLower.Contains("suite") || roomNameLower.Contains("luxury") || 
                roomNameLower.Contains("premium") || roomNameLower.Contains("executive"))
            {
                return RoomFeatures.LuxuryFeatures();
            }
            
            // Standard features for regular rooms
            return RoomFeatures.StandardFeatures();
        }
    }
}