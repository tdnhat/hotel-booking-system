using HotelBookingSystem.BookingService.Application.DTOs;
using HotelBookingSystem.BookingService.Domain.Enums;
using HotelBookingSystem.BookingService.Domain.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.BookingService.Application.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingResponse>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CreateBookingCommandHandler> _logger;

        public CreateBookingCommandHandler(IPublishEndpoint publishEndpoint, ILogger<CreateBookingCommandHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        public async Task<BookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var bookingId = Guid.NewGuid();

            await _publishEndpoint.Publish<BookingRequested>(new
            {
                BookingId = bookingId,
                RoomTypeId = request.RoomTypeId,
                HotelId = request.HotelId,
                GuestEmail = request.GuestEmail,
                TotalPrice = request.TotalPrice,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                NumberOfGuests = request.NumberOfGuests,
                RequestAt = DateTime.UtcNow
            }, cancellationToken);

            _logger.LogInformation("BookingRequested event published for BookingId: {BookingId}", bookingId);

            return new BookingResponse
            {
                BookingId = bookingId,
                CorrelationId = Guid.NewGuid(),
                Status = BookingStatus.Submitted,
                RoomTypeId = request.RoomTypeId,
                HotelId = request.HotelId,
                GuestEmail = request.GuestEmail,
                TotalPrice = request.TotalPrice,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                NumberOfGuests = request.NumberOfGuests,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
        }
    }
}