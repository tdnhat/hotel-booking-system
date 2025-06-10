using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HotelBookingSystem.RoomManagementService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Register AutoMapper if you plan to use it
            // services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register other application services here as they grow
            // services.AddScoped<IApplicationService, ApplicationService>();

            return services;
        }
    }
}