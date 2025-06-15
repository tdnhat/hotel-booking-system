using HotelBookingSystem.RoomManagementService.Application.Common.Models;
using HotelBookingSystem.RoomManagementService.Application.DTOs;
using HotelBookingSystem.RoomManagementService.Domain.Entities;
using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;
using MediatR;

namespace HotelBookingSystem.RoomManagementService.Application.Hotels.Commands.CreateHotel
{
    public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, Result<HotelDto>>
    {
        private readonly IHotelRepository _hotelRepository;

        public CreateHotelCommandHandler(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task<Result<HotelDto>> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
        {
            var existingHotel = await _hotelRepository.ExistsByNameAsync(request.Name, cancellationToken);

            if (existingHotel)
            {
                return Result.Failure<HotelDto>($"Hotel with name {request.Name} already exists.");
            }

            var address = new Address(
                request.Address.Street,
                request.Address.City,
                request.Address.State,
                request.Address.Country,
                request.Address.PostalCode
            );

            var hotel = new Hotel(
                HotelId.New(),
                request.Name,
                address,
                request.Description,
                request.StarRating,
                request.ContactEmail,
                request.ContactPhone
            );

            await _hotelRepository.AddAsync(hotel, cancellationToken);

            var hotelDto = MapToDto(hotel);

            return Result.Success(hotelDto);
        }

        private static HotelDto MapToDto(Hotel hotel)
        {
            return new HotelDto
            {
                Id = hotel.Id.Value,
                Name = hotel.Name,
                Description = hotel.Description,
                Status = hotel.Status,
                StarRating = hotel.StarRating,
                ContactEmail = hotel.ContactEmail,
                ContactPhone = hotel.ContactPhone,
                CreatedAt = hotel.CreatedAt,
                UpdatedAt = hotel.UpdatedAt,
                Address = new AddressDto
                {
                    Street = hotel.Address.Street,
                    City = hotel.Address.City,
                    State = hotel.Address.State,
                    Country = hotel.Address.Country,
                    PostalCode = hotel.Address.PostalCode
                },
                RoomTypes = hotel.RoomTypes.Select(rt => new RoomTypeDto
                {
                    Id = rt.Id.Value,
                    Name = rt.Name,
                    Description = rt.Description,
                    MaxOccupancy = rt.MaxOccupancy,
                    BasePrice = rt.BasePrice.Amount,
                    Currency = rt.BasePrice.Currency,
                    Status = rt.Status,
                    BedConfiguration = new BedConfigurationDto
                    {
                        PrimaryBedType = rt.BedConfiguration.PrimaryBedType,
                        PrimaryBedCount = rt.BedConfiguration.PrimaryBedCount,
                        SecondaryBedType = rt.BedConfiguration.SecondaryBedType,
                        SecondaryBedCount = rt.BedConfiguration.SecondaryBedCount,
                        Description = rt.BedConfiguration.GetDescription()
                    },
                    Features = rt.Features?.Features.ToList() ?? new List<string>()
                }).ToList()
            };
        }
    }
}