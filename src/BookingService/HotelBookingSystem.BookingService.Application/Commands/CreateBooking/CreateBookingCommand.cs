using HotelBookingSystem.BookingService.Application.DTOs;
using MediatR;

namespace HotelBookingSystem.BookingService.Application.Commands.CreateBooking
{
    public record CreateBookingCommand(
        Guid RoomTypeId,
        Guid HotelId,
        string GuestEmail,
        decimal TotalPrice,
        DateTime CheckInDate,
        DateTime CheckOutDate,
        int NumberOfGuests
    ) : IRequest<BookingResponse>;
}