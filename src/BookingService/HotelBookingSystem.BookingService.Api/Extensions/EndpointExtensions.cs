using HotelBookingSystem.BookingService.Api.Endpoints;

namespace HotelBookingSystem.BookingService.Api.Extensions
{
    public static class EndpointExtensions
    {
        public static WebApplication MapBookingEndpoints(this WebApplication app)
        {
            var apiGroup = app.MapGroup("/api");

            var v1Group = apiGroup.MapGroup("/v1").WithTags("Booking Service API v1");
            BookingEndpoints.MapBookingEndpoints(v1Group);

            return app;
        }

        // Future use when you need multiple API versions
        public static WebApplication MapVersionedEndpoints(this WebApplication app)
        {
            var apiGroup = app.MapGroup("/api");

            // Version 1 endpoints
            var v1Group = apiGroup.MapGroup("/v1").WithTags("Booking Service API v1");
            BookingEndpoints.MapBookingEndpoints(v1Group);

            // Version 2 endpoints (future implementation)
            var v2Group = apiGroup.MapGroup("/v2").WithTags("Booking Service API v2");
            // TODO: Implement when v2 endpoints are needed

            return app;
            
        }
    }
}