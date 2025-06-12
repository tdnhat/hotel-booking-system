using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using HotelBookingSystem.InventoryService.Domain.Enums;

namespace HotelBookingSystem.InventoryService.Application.Services
{
    public class RoomInventoryService : IRoomInventoryService
    {
        private readonly ILogger<RoomInventoryService> _logger;
        private readonly IRoomAvailabilityRepository _availabilityRepository;
        private readonly IRoomHoldRepository _holdRepository;

        public RoomInventoryService(
            ILogger<RoomInventoryService> logger,
            IRoomAvailabilityRepository availabilityRepository,
            IRoomHoldRepository holdRepository)
        {
            _logger = logger;
            _availabilityRepository = availabilityRepository;
            _holdRepository = holdRepository;
        }

        public async Task<(bool Success, string? HoldReference, string? ErrorMessage)> HoldRoomAsync(
            Guid bookingId, 
            Guid roomTypeId, 
            DateTime checkInDate, 
            DateTime checkOutDate, 
            TimeSpan holdDuration)
        {
            _logger.LogInformation("Attempting to hold room for booking {BookingId}", bookingId);

            // Check if room is available for the entire period
            var isAvailable = await IsRoomAvailableAsync(roomTypeId, checkInDate, checkOutDate);
            if (!isAvailable)
            {
                _logger.LogWarning("Room {RoomTypeId} not available for booking {BookingId}", roomTypeId, bookingId);
                return (false, null, "No rooms available for the requested dates");
            }

            // Check if this booking already has a hold
            var existingHold = await _holdRepository.GetActiveHoldByBookingIdAsync(bookingId);
            if (existingHold != null)
            {
                _logger.LogInformation("Booking {BookingId} already has an active hold: {HoldReference}", 
                    bookingId, existingHold.HoldReference);
                return (true, existingHold.HoldReference, null);
            }

            // Create room hold
            var holdReference = GenerateHoldReference(bookingId);
            var expiresAt = DateTime.UtcNow.Add(holdDuration);

            var roomHold = new RoomHold
            {
                Id = Guid.NewGuid(),
                HoldReference = holdReference,
                BookingId = bookingId,
                RoomTypeId = roomTypeId,
                StartDate = checkInDate,
                EndDate = checkOutDate,
                ExpiresAt = expiresAt
            };

            // Update room availability status
            var dateRange = GetDateRange(checkInDate, checkOutDate);
            await _availabilityRepository.UpdateRoomStatusAsync(roomTypeId, dateRange, AvailabilityStatus.Held, bookingId);
            
            // Save the hold
            await _holdRepository.CreateAsync(roomHold);

            _logger.LogInformation("Successfully created hold {HoldReference} for booking {BookingId}", 
                holdReference, bookingId);

            return (true, holdReference, null);
        }

        public async Task<bool> ReleaseRoomAsync(Guid bookingId, string holdReference)
        {
            _logger.LogInformation("Attempting to release room hold {HoldReference} for booking {BookingId}", 
                holdReference, bookingId);

            var hold = await _holdRepository.GetByHoldReferenceAsync(holdReference);
            if (hold == null)
            {
                _logger.LogWarning("Hold {HoldReference} not found for booking {BookingId}", holdReference, bookingId);
                return true; // Consider it successfully released if hold doesn't exist
            }

            if (hold.IsReleased)
            {
                _logger.LogInformation("Hold {HoldReference} for booking {BookingId} was already released", 
                    holdReference, bookingId);
                return true; // Already released
            }

            // Release the hold
            hold.IsReleased = true;
            hold.ReleasedAt = DateTime.UtcNow;
            await _holdRepository.UpdateAsync(hold);

            // Update room availability back to available
            var dateRange = GetDateRange(hold.StartDate, hold.EndDate);
            await _availabilityRepository.UpdateRoomStatusAsync(hold.RoomTypeId, dateRange, AvailabilityStatus.Available, null);

            _logger.LogInformation("Successfully released hold {HoldReference} for booking {BookingId}", 
                holdReference, bookingId);

            return true;
        }

        public async Task<bool> IsRoomAvailableAsync(Guid roomTypeId, DateTime checkInDate, DateTime checkOutDate)
        {
            var dateRange = GetDateRange(checkInDate, checkOutDate);
            return await _availabilityRepository.IsRoomAvailableAsync(roomTypeId, dateRange);
        }

        private static string GenerateHoldReference(Guid bookingId)
        {
            return $"HOLD-{bookingId.ToString("N")[..8]}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        private static List<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
        {
            var dates = new List<DateTime>();
            for (var date = startDate.Date; date < endDate.Date; date = date.AddDays(1))
            {
                dates.Add(date);
            }
            return dates;
        }
    }
} 