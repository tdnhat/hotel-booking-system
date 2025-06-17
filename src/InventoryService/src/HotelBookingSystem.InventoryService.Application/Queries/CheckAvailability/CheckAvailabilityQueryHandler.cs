using HotelBookingSystem.InventoryService.Application.Common.Models;
using HotelBookingSystem.InventoryService.Application.DTOs;
using HotelBookingSystem.InventoryService.Domain.Repositories;
using HotelBookingSystem.InventoryService.Domain.Services;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.InventoryService.Application.Queries.CheckAvailability;

public class CheckAvailabilityQueryHandler : IRequestHandler<CheckAvailabilityQuery, Result<DateRangeAvailabilityDto>>
{
    private readonly IRoomInventoryRepository _roomInventoryRepository;
    private readonly IInventoryDomainService _inventoryDomainService;
    private readonly ILogger<CheckAvailabilityQueryHandler> _logger;

    public CheckAvailabilityQueryHandler(
        IRoomInventoryRepository roomInventoryRepository,
        IInventoryDomainService inventoryDomainService,
        ILogger<CheckAvailabilityQueryHandler> logger)
    {
        _roomInventoryRepository = roomInventoryRepository;
        _inventoryDomainService = inventoryDomainService;
        _logger = logger;
    }

    public async Task<Result<DateRangeAvailabilityDto>> Handle(CheckAvailabilityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Checking availability for hotel {HotelId}, room type {RoomTypeId} from {CheckIn} to {CheckOut}", 
                request.HotelId, request.RoomTypeId, request.CheckIn, request.CheckOut);

            var hotelId = HotelId.From(request.HotelId);
            var roomTypeId = RoomTypeId.From(request.RoomTypeId);
            var dateRange = new DateRange(request.CheckIn, request.CheckOut);

            // Check overall availability
            var isAvailable = await _inventoryDomainService.CanHoldRoomsAsync(
                hotelId, roomTypeId, dateRange, request.RequestedRooms, cancellationToken);

            // Get daily availability details
            var dailyInventory = await _roomInventoryRepository.GetByRoomTypeAndDateRangeAsync(
                hotelId, roomTypeId, dateRange, cancellationToken);

            var dailyAvailability = new List<RoomAvailabilityDto>();
            decimal totalAmount = 0;

            foreach (var date in dateRange.GetDates())
            {
                var inventory = dailyInventory.FirstOrDefault(i => i.Date.Date == date.Date);
                if (inventory != null)
                {
                    dailyAvailability.Add(new RoomAvailabilityDto
                    {
                        HotelId = request.HotelId,
                        RoomTypeId = request.RoomTypeId,
                        Date = date,
                        AvailableRooms = inventory.AvailableRooms,
                        TotalRooms = inventory.TotalRooms,
                        CurrentPrice = inventory.CurrentPrice.Amount,
                        Currency = inventory.CurrentPrice.Currency.ToString(),
                        IsAvailable = inventory.AvailableRooms >= request.RequestedRooms
                    });

                    totalAmount += inventory.CurrentPrice.Amount * request.RequestedRooms;
                }
                else
                {
                    // No inventory record for this date means no availability
                    dailyAvailability.Add(new RoomAvailabilityDto
                    {
                        HotelId = request.HotelId,
                        RoomTypeId = request.RoomTypeId,
                        Date = date,
                        AvailableRooms = 0,
                        TotalRooms = 0,
                        CurrentPrice = 0,
                        Currency = "USD",
                        IsAvailable = false
                    });
                }
            }

            var result = new DateRangeAvailabilityDto
            {
                HotelId = request.HotelId,
                RoomTypeId = request.RoomTypeId,
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                RequestedRooms = request.RequestedRooms,
                IsAvailable = isAvailable,
                TotalAmount = totalAmount,
                Currency = dailyAvailability.FirstOrDefault()?.Currency ?? "USD",
                DailyAvailability = dailyAvailability
            };

            _logger.LogInformation("Availability check completed. Available: {IsAvailable}, Total amount: {TotalAmount}", 
                isAvailable, totalAmount);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking availability for hotel {HotelId}, room type {RoomTypeId}", 
                request.HotelId, request.RoomTypeId);
            return Result.Failure<DateRangeAvailabilityDto>($"Failed to check availability: {ex.Message}");
        }
    }
} 