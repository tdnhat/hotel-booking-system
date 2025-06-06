using HotelBookingSystem.RoomManagementService.Api.Endpoints;

namespace HotelBookingSystem.RoomManagementService.Api.Extensions
{
    public static class EndpointExtensions
    {
        public static WebApplication MapRoomManagementEndpoints(this WebApplication app)
        {
            RoomManagementEndpoints.MapRoomManagementEndpoints(app);

            return app;
        }

        public static WebApplication MapVersionedEndpoints(this WebApplication app)
        {
            var v1 = app.MapGroup("/api/v1");
            var v2 = app.MapGroup("/api/v2");

            // Version 1 endpoints
            RoomManagementEndpoints.MapRoomManagementEndpoints(v1);

            return app;
        }
    }

}