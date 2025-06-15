using HotelBookingSystem.RoomManagementService.Application.Common.Exceptions;
using HotelBookingSystem.RoomManagementService.Application.Common.Models;
using HotelBookingSystem.RoomManagementService.Application.DTOs;
using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;
using MediatR;

namespace HotelBookingSystem.RoomManagementService.Application.Hotels.Queries.GetHotel
{
    public class GetHotelQueryHandler : IRequestHandler<GetHotelQuery, Result<HotelDto>>
    {
        private readonly IHotelRepository _hotelRepository;

        public GetHotelQueryHandler(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task<Result<HotelDto>> Handle(GetHotelQuery request, CancellationToken cancellationToken)
        {
            var hotelId = HotelId.From(request.HotelId);
            var hotel = await _hotelRepository.GetByIdWithRoomTypesAsync(hotelId, cancellationToken);

            if (hotel is null)
            {
                throw new NotFoundException(nameof(hotel), request.HotelId);
            }

            var hotelDto = new HotelDto
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
                    Id = rt.Id?.Value ?? Guid.Empty,
                    Name = rt.Name ?? string.Empty,
                    Description = rt.Description ?? string.Empty,
                    MaxOccupancy = rt.MaxOccupancy,
                    BasePrice = rt.BasePrice?.Amount ?? 0,
                    Currency = rt.BasePrice?.Currency ?? Domain.Enums.Currency.USD,
                    Status = rt.Status,
                    BedConfiguration = rt.BedConfiguration != null ? new BedConfigurationDto
                    {
                        PrimaryBedType = rt.BedConfiguration.PrimaryBedType,
                        PrimaryBedCount = rt.BedConfiguration.PrimaryBedCount,
                        SecondaryBedType = rt.BedConfiguration.SecondaryBedType,
                        SecondaryBedCount = rt.BedConfiguration.SecondaryBedCount,
                        Description = rt.BedConfiguration.GetDescription()
                    } : new BedConfigurationDto(),
                    Features = rt.Features?.Features?.ToList() ?? new List<string>()
                }).ToList()
            };

            return Result.Success(hotelDto);
        }
    }
}