using HotelBookingSystem.BookingService.Application.Commands.CreateBooking;
using HotelBookingSystem.BookingService.Application.DTOs;
using HotelBookingSystem.BookingService.Application.Queries.GetBookingStatus;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.BookingService.Api.Endpoints
{
    public static class BookingEndpoints
    {
        public static void MapBookingEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/bookings")
                .WithTags("Booking Management")
                .WithOpenApi();

            group.MapPost("/", CreateBooking)
                .Produces<BookingResponse>(StatusCodes.Status201Created)
                .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithName("CreateBooking")
                .WithSummary("Create a new booking")
                .WithDescription("Creates a new booking for a guest");

            group.MapGet("/{bookingId}/status", GetBookingStatus)
                .Produces<BookingStatusResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetBookingStatus")
                .WithSummary("Get the status of a booking")
                .WithDescription("Retrieves the current status of a booking");
        }
        private static async Task<IResult> CreateBooking(
            CreateBookingRequest request,
            ISender sender,
            ILogger<CreateBookingCommand> logger,
            CancellationToken cancellationToken)
        {
            // Validate request
            if (!request.IsValidDateRange())
            {
                return Results.BadRequest(new {
                    Title = "Invalid Date Range",
                    Detail = "Check-in date must be today or later, and check-out date must be after check-in date"
                });
            }

            logger.LogInformation("Creating booking for room type {RoomTypeId} at hotel {HotelId} for {GuestEmail}",
                request.RoomTypeId,
                request.HotelId,
                request.GuestEmail);

            var command = new CreateBookingCommand(
                request.RoomTypeId,
                request.HotelId,
                request.GuestEmail,
                request.TotalPrice,
                request.CheckInDate,
                request.CheckOutDate,
                request.NumberOfGuests
            );

            var result = await sender.Send(command, cancellationToken);

            logger.LogInformation("Booking created successfully with ID {BookingId}", result.BookingId);

            return Results.Accepted($"/bookings/{result.BookingId}/status", result);
        }

        private static async Task<IResult> GetBookingStatus(
            [FromRoute] Guid bookingId,
            ISender sender,
            ILogger<CreateBookingCommand> logger,
            CancellationToken cancellationToken)
        {
            var query = new GetBookingStatusQuery(bookingId);

            var result = await sender.Send(query, cancellationToken);

            return result is not null ?
                Results.Ok(result) :
                Results.NotFound();
        }
    }
}