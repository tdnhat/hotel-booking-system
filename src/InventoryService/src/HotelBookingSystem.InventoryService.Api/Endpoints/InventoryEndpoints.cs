using HotelBookingSystem.InventoryService.Application.Commands.HoldRoom;
using HotelBookingSystem.InventoryService.Application.Commands.ReleaseRoomHold;
using HotelBookingSystem.InventoryService.Application.Commands.ConfirmRoomBooking;
using HotelBookingSystem.InventoryService.Application.Queries.CheckAvailability;
using HotelBookingSystem.InventoryService.Application.Queries.GetRoomHold;
using HotelBookingSystem.InventoryService.Api.Endpoints.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.InventoryService.Api.Endpoints;

public static class InventoryEndpoints
{
    public static void MapInventoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/inventory")
            .WithTags("Inventory Management")
            .WithOpenApi();

        // Availability endpoints
        group.MapPost("/availability/check", CheckAvailability)
            .WithName("CheckRoomAvailability")
            .WithSummary("Check room availability for specific dates")
            .WithDescription("Checks availability and pricing for a room type on specified dates")
            .Produces<CheckAvailabilityResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        // Hold management endpoints
        group.MapPost("/holds", CreateRoomHold)
            .WithName("CreateRoomHold")
            .WithSummary("Create a temporary room hold")
            .WithDescription("Creates a temporary hold on rooms for a booking")
            .Produces<CreateRoomHoldResponse>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);

        group.MapGet("/holds/{holdId:guid}", GetRoomHold)
            .WithName("GetRoomHold")
            .WithSummary("Get room hold details")
            .WithDescription("Retrieves details of a specific room hold")
            .Produces<GetRoomHoldResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/holds/{holdId:guid}/release", ReleaseRoomHold)
            .WithName("ReleaseRoomHold")
            .WithSummary("Release a room hold")
            .WithDescription("Releases a room hold, making the rooms available again")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("/holds/{holdId:guid}/confirm", ConfirmRoomBooking)
            .WithName("ConfirmRoomBooking")
            .WithSummary("Confirm a room booking")
            .WithDescription("Converts a room hold into a confirmed booking")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CheckAvailability(
        [FromBody] CheckAvailabilityRequest request,
        ISender sender,
        ILogger<CheckAvailabilityQuery> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking availability for hotel {HotelId}, room type {RoomTypeId}, dates {CheckIn} to {CheckOut}",
            request.HotelId, request.RoomTypeId, request.CheckIn, request.CheckOut);

        var query = new CheckAvailabilityQuery
        {
            HotelId = request.HotelId,
            RoomTypeId = request.RoomTypeId,
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            RequestedRooms = request.RoomCount
        };

        var result = await sender.Send(query, cancellationToken);
        
        if (!result.IsSuccess)
            return Results.BadRequest(new { Title = "Availability check failed", Detail = result.Error });

        var response = new CheckAvailabilityResponse
        {
            IsAvailable = result.Value.IsAvailable,
            AvailableRooms = result.Value.IsAvailable ? request.RoomCount : 0,
            PricePerNight = result.Value.TotalAmount / (request.CheckOut - request.CheckIn).Days,
            Currency = result.Value.Currency,
            TotalPrice = result.Value.TotalAmount,
            Message = result.Value.IsAvailable ? "Rooms available" : "Insufficient availability"
        };

        return Results.Ok(response);
    }

    private static async Task<IResult> CreateRoomHold(
        [FromBody] CreateRoomHoldRequest request,
        ISender sender,
        ILogger<HoldRoomCommand> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating room hold for booking {BookingId}", request.BookingId);

        var command = new HoldRoomCommand
        {
            BookingId = request.BookingId,
            HotelId = request.HotelId,
            RoomTypeId = request.RoomTypeId,
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            RoomCount = request.RoomCount,
            HoldDuration = request.HoldDuration
        };

        var result = await sender.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
            return Results.BadRequest(new { Title = "Hold creation failed", Detail = result.Error });

        var response = new CreateRoomHoldResponse
        {
            Id = result.Value.Id,
            BookingId = result.Value.BookingId,
            Status = result.Value.Status,
            ExpiresAt = result.Value.ExpiresAt,
            HoldReference = result.Value.HoldReference
        };

        return Results.Created($"/api/v1/inventory/holds/{result.Value.Id}", response);
    }

    private static async Task<IResult> GetRoomHold(
        Guid holdId,
        ISender sender,
        ILogger<GetRoomHoldQuery> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting room hold {HoldId}", holdId);

        var query = new GetRoomHoldQuery { HoldId = holdId };
        var result = await sender.Send(query, cancellationToken);
        
        if (!result.IsSuccess)
            return Results.NotFound(new { Title = "Hold not found", Detail = result.Error });

        var response = new GetRoomHoldResponse
        {
            Id = result.Value.Id,
            BookingId = result.Value.BookingId,
            HotelId = result.Value.HotelId,
            RoomTypeId = result.Value.RoomTypeId,
            CheckIn = result.Value.CheckIn,
            CheckOut = result.Value.CheckOut,
            RoomCount = result.Value.RoomCount,
            Status = result.Value.Status,
            CreatedAt = result.Value.CreatedAt,
            ExpiresAt = result.Value.ExpiresAt,
            HoldReference = result.Value.HoldReference
        };

        return Results.Ok(response);
    }

    private static async Task<IResult> ReleaseRoomHold(
        Guid holdId,
        [FromBody] ReleaseRoomHoldRequest request,
        ISender sender,
        ILogger<ReleaseRoomHoldCommand> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Releasing room hold {HoldId}", holdId);

        var command = new ReleaseRoomHoldCommand
        {
            BookingId = request.BookingId,
            HoldId = holdId,
            Reason = request.Reason ?? "Manual release"
        };

        var result = await sender.Send(command, cancellationToken);
        
        return result.IsSuccess 
            ? Results.NoContent()
            : Results.BadRequest(new { Title = "Hold release failed", Detail = result.Error });
    }

    private static async Task<IResult> ConfirmRoomBooking(
        Guid holdId,
        [FromBody] ConfirmRoomBookingRequest request,
        ISender sender,
        ILogger<ConfirmRoomBookingCommand> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Confirming room booking for hold {HoldId}", holdId);

        var command = new ConfirmRoomBookingCommand
        {
            BookingId = request.BookingId,
            HoldId = holdId,
            PaymentId = request.PaymentId
        };

        var result = await sender.Send(command, cancellationToken);
        
        return result.IsSuccess 
            ? Results.NoContent()
            : Results.BadRequest(new { Title = "Booking confirmation failed", Detail = result.Error });
    }
}

// Request/Response models for API contracts
public record CheckAvailabilityRequest
{
    public Guid HotelId { get; init; }
    public Guid RoomTypeId { get; init; }
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }
    public int RoomCount { get; init; } = 1;
}

public record CreateRoomHoldRequest
{
    public Guid BookingId { get; init; }
    public Guid HotelId { get; init; }
    public Guid RoomTypeId { get; init; }
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }
    public int RoomCount { get; init; } = 1;
    public TimeSpan HoldDuration { get; init; } = TimeSpan.FromMinutes(15);
}

public record ReleaseRoomHoldRequest
{
    public Guid BookingId { get; init; }
    public string? Reason { get; init; }
}

public record ConfirmRoomBookingRequest
{
    public Guid BookingId { get; init; }
    public Guid PaymentId { get; init; }
} 