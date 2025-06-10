using HotelBookingSystem.BookingService.Application.DTOs;
using HotelBookingSystem.BookingService.Application.Interfaces;
using HotelBookingSystem.BookingService.Domain.Enums;
using MediatR;

namespace HotelBookingSystem.BookingService.Application.Queries.GetBookingStatus
{
    public class GetBookingStatusQueryHandler : IRequestHandler<GetBookingStatusQuery, BookingStatusResponse?>
    {
        private readonly ISagaRepository _sagaRepository;
        public GetBookingStatusQueryHandler(ISagaRepository sagaRepository)
        {
            _sagaRepository = sagaRepository;
        }

        public async Task<BookingStatusResponse?> Handle(GetBookingStatusQuery request, CancellationToken cancellationToken)
        {
            var booking = await _sagaRepository.GetByBookingIdAsync(request.BookingId, cancellationToken);

            if (booking == null)
            {
                return null;
            }

            var status = booking.Status;
            var numberOfNights = (booking.CheckOutDate - booking.CheckInDate).Days;
            var pricePerNight = numberOfNights > 0 ? booking.TotalPrice / numberOfNights : booking.TotalPrice;

            return new BookingStatusResponse
            {
                BookingId = request.BookingId,
                Status = status,
                StatusName = status.ToString(),
                LastUpdated = booking.UpdatedAt,
                FailureReason = booking.FailureReason,
                IsCompleted = IsCompletedStatus(status),
                IsSuccess = status == BookingStatus.Confirmed,
                StatusMessage = GetStatusMessage(status),
                ProgressPercentage = GetProgressPercentage(status),

                // Booking Details
                BookingDetails = new BookingDetailsDto
                {
                    HotelId = booking.HotelId,
                    RoomTypeId = booking.RoomTypeId,
                    GuestEmail = booking.GuestEmail,
                    TotalPrice = booking.TotalPrice,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    NumberOfGuests = booking.NumberOfGuests,
                    NumberOfNights = numberOfNights,
                    PricePerNight = pricePerNight
                }
            };
        }

        private static bool IsCompletedStatus(BookingStatus status)
        {
            return status == BookingStatus.Confirmed || status == BookingStatus.Failed;
        }

        private static string GetStatusMessage(BookingStatus status)
        {
            return status switch
            {
                BookingStatus.Submitted => "Booking request received and being processed",
                BookingStatus.RoomHoldRequested => "Checking room availability",
                BookingStatus.RoomHeld => "Room held successfully, processing payment",
                BookingStatus.PaymentProcessing => "Processing payment",
                BookingStatus.RoomReleaseRequested => "Payment failed, releasing room hold",
                BookingStatus.Confirmed => "Booking confirmed successfully!",
                BookingStatus.Failed => "Booking failed",
                _ => "Unknown status"
            };
        }

        private static int GetProgressPercentage(BookingStatus status)
        {
            return status switch
            {
                BookingStatus.Submitted => 10,
                BookingStatus.RoomHoldRequested => 30,
                BookingStatus.RoomHeld => 60,
                BookingStatus.PaymentProcessing => 80,
                BookingStatus.Confirmed => 100,
                BookingStatus.Failed => 100,
                BookingStatus.RoomReleaseRequested => 90,
                _ => 0
            };
        }
    }
}