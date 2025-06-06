using HotelBookingSystem.RoomManagementService.Api.Endpoints;

namespace HotelBookingSystem.RoomManagementService.Api.Extensions
{
    public static class EndpointExtensions
    {
        public static WebApplication MapRoomManagementEndpoints(this WebApplication app)
        {
            var apiGroup = app.MapGroup("/api");

            var v1Group = apiGroup.MapGroup("/v1").WithTags("Room Management API v1");
            RoomManagementEndpoints.MapRoomManagementEndpoints(v1Group);

            return app;
        }

        // Future use when you need multiple API versions
        public static WebApplication MapVersionedEndpoints(this WebApplication app)
        {
            var apiGroup = app.MapGroup("/api");

            // Version 1 endpoints
            var v1Group = apiGroup.MapGroup("/v1").WithTags("Room Management API v1");
            RoomManagementEndpoints.MapRoomManagementEndpoints(v1Group);

            // Version 2 endpoints (future implementation)
            var v2Group = apiGroup.MapGroup("/v2").WithTags("Room Management API v2");
            // TODO: Implement when v2 endpoints are needed
            // RoomManagementEndpoints.MapRoomManagementV2Endpoints(v2Group);

            return app;
        }
    }

}