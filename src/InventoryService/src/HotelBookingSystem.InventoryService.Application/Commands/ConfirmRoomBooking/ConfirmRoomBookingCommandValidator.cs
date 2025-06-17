using FluentValidation;

namespace HotelBookingSystem.InventoryService.Application.Commands.ConfirmRoomBooking;

public class ConfirmRoomBookingCommandValidator : AbstractValidator<ConfirmRoomBookingCommand>
{
    public ConfirmRoomBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required");

        RuleFor(x => x.HoldId)
            .NotEmpty()
            .WithMessage("Hold ID is required");

        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage("Payment ID is required");
    }
} 