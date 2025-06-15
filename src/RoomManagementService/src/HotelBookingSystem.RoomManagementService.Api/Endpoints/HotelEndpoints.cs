using HotelBookingSystem.RoomManagementService.Application.Hotels.Commands.CreateHotel;
using HotelBookingSystem.RoomManagementService.Application.Hotels.Queries.GetHotel;
using HotelBookingSystem.RoomManagementService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.RoomManagementService.Api.Endpoints
{
    public static class HotelEndpoints
    {
        public static void MapHotelEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/hotels")
                .WithTags("Hotel Management")
                .WithOpenApi();

            group.MapPost("/", CreateHotel)
                .Produces<HotelDto>(StatusCodes.Status201Created)
                .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithName("CreateHotel")
                .WithSummary("Create a new hotel")
                .WithDescription("Creates a new hotel in the system");

            group.MapGet("/{hotelId}", GetHotel)
                .Produces<HotelDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetHotel")
                .WithSummary("Get a hotel by ID")
                .WithDescription("Retrieves hotel information by its ID");
        }

        private static async Task<IResult> CreateHotel(
            CreateHotelCommand command,
            ISender sender,
            ILogger<CreateHotelCommand> logger,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Creating hotel with name {HotelName}", command.Name);

            var result = await sender.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                logger.LogWarning("Failed to create hotel: {Error}", result.Error);
                return Results.BadRequest(new { 
                    Title = "Hotel creation failed", 
                    Detail = result.Error 
                });
            }

            logger.LogInformation("Hotel created successfully with ID {HotelId}", result.Value.Id);

            return Results.Created($"/hotels/{result.Value.Id}", result.Value);
        }

        private static async Task<IResult> GetHotel(
            [FromRoute] Guid hotelId,
            ISender sender,
            ILogger<GetHotelQuery> logger,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting hotel with ID {HotelId}", hotelId);

            var query = new GetHotelQuery(hotelId);
            var result = await sender.Send(query, cancellationToken);

            if (!result.IsSuccess)
            {
                logger.LogInformation("Hotel with ID {HotelId} not found", hotelId);
                return Results.NotFound();
            }

            return Results.Ok(result.Value);
        }
    }
} 