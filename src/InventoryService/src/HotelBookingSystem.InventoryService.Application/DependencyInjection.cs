using HotelBookingSystem.InventoryService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBookingSystem.InventoryService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IRoomInventoryService, RoomInventoryService>();

            return services;
        }
    }
} 