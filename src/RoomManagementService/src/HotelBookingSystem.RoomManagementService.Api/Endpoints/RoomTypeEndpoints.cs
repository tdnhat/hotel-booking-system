using HotelBookingSystem.RoomManagementService.Application.RoomTypes.Commands.AddRoomType;
using HotelBookingSystem.RoomManagementService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.RoomManagementService.Api.Endpoints
{
    public static class RoomTypeEndpoints
    {
        public static void MapRoomTypeEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/room-types")
                .WithTags("Room Type Management")
                .WithOpenApi();

            group.MapPost("/", AddRoomType)
                .Produces<RoomTypeDto>(StatusCodes.Status201Created)
                .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithName("AddRoomType")
                .WithSummary("Add a new room type to a hotel")
                .WithDescription("Adds a new room type configuration to an existing hotel");
        }

        private static async Task<IResult> AddRoomType(
            AddRoomTypeCommand command,
            ISender sender,
            ILogger<AddRoomTypeCommand> logger,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Adding room type {RoomTypeName} to hotel {HotelId}", 
                command.Name, command.HotelId);

            var result = await sender.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                logger.LogWarning("Failed to add room type to hotel {HotelId}: {Error}", 
                    command.HotelId, result.Error);
                return Results.BadRequest(new { 
                    Title = "Room type creation failed", 
                    Detail = result.Error 
                });
            }

            logger.LogInformation("Room type created successfully with ID {RoomTypeId}", 
                result.Value.Id);

            return Results.Created($"/room-types/{result.Value.Id}", result.Value);
        }
    }
} 