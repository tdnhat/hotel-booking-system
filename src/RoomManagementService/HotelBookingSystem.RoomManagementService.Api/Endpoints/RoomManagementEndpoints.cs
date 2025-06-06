using HotelBookingSystem.RoomManagementService.Application.DTOs;
using HotelBookingSystem.RoomManagementService.Application.Queries.GetRoomTypeById;
using HotelBookingSystem.RoomManagementService.Application.Queries.GetRoomTypes;
using MediatR;

namespace HotelBookingSystem.RoomManagementService.Api.Endpoints
{
    public static class RoomManagementEndpoints
    {
        public static void MapRoomManagementEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/rooms")
                .WithTags("Room Management")
                .WithOpenApi();

            // GET /roomTypes - Get all room types
            group.MapGet("/roomTypes", GetRoomTypes)
                .Produces<IEnumerable<RoomTypeDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetRoomTypes")
                .WithSummary("Get all room types")
                .WithDescription("Retrieves all room types from the database");

            // GET /roomTypes/{id} - Get a room type by ID
            group.MapGet("/roomTypes/{id}", GetRoomTypeById)
                .Produces<RoomTypeDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetRoomTypeById")
                .WithSummary("Get a room type by ID")
                .WithDescription("Retrieves a room type by its unique identifier");
        }

        private static async Task<IResult> GetRoomTypes(ISender sender)
        {
            var query = new GetRoomTypesQuery();
            var result = await sender.Send(query);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        }

        private static async Task<IResult> GetRoomTypeById(Guid id, ISender sender)
        {
            var query = new GetRoomTypeByIdQuery(id);
            var result = await sender.Send(query);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        }
    }
}