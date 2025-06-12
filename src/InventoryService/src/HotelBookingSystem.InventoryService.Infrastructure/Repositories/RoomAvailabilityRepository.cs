using HotelBookingSystem.InventoryService.Application.Interfaces;
using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.InventoryService.Infrastructure.Repositories
{
    public class RoomAvailabilityRepository : IRoomAvailabilityRepository
    {
        private readonly InventoryDbContext _context;

        public RoomAvailabilityRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsRoomAvailableAsync(Guid roomTypeId, List<DateTime> dates)
        {
            // Check if all dates are available
            var unavailableDates = await _context.RoomAvailabilities
                .Where(ra => ra.RoomTypeId == roomTypeId && 
                            dates.Contains(ra.Date) && 
                            ra.Status != AvailabilityStatus.Available)
                .CountAsync();

            return unavailableDates == 0;
        }

        public async Task UpdateRoomStatusAsync(Guid roomTypeId, List<DateTime> dates, AvailabilityStatus status, Guid? bookingId)
        {
            foreach (var date in dates)
            {
                var availability = await _context.RoomAvailabilities
                    .FirstOrDefaultAsync(ra => ra.RoomTypeId == roomTypeId && ra.Date == date);

                if (availability == null)
                {
                    // Create new availability record
                    availability = new RoomAvailability
                    {
                        Id = Guid.NewGuid(),
                        RoomTypeId = roomTypeId,
                        Date = date,
                        Status = status,
                        BookingId = bookingId
                    };
                    _context.RoomAvailabilities.Add(availability);
                }
                else
                {
                    // Update existing record
                    availability.Status = status;
                    availability.BookingId = bookingId;
                    availability.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
} 