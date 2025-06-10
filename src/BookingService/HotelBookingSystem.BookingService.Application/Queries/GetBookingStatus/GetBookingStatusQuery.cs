using HotelBookingSystem.BookingService.Application.DTOs;
using MediatR;

namespace HotelBookingSystem.BookingService.Application.Queries.GetBookingStatus
{
    public record GetBookingStatusQuery(Guid BookingId) : IRequest<BookingStatusResponse?>;
}