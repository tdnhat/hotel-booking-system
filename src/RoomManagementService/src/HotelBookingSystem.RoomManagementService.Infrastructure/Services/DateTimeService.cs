using HotelBookingSystem.RoomManagementService.Application.Common.Abstractions;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    }
} 