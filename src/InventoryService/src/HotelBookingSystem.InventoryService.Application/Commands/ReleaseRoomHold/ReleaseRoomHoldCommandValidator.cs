using FluentValidation;

namespace HotelBookingSystem.InventoryService.Application.Commands.ReleaseRoomHold;

public class ReleaseRoomHoldCommandValidator : AbstractValidator<ReleaseRoomHoldCommand>
{
    public ReleaseRoomHoldCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required");

        RuleFor(x => x.HoldId)
            .NotEmpty()
            .WithMessage("Hold ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Release reason is required")
            .MaximumLength(200)
            .WithMessage("Release reason cannot exceed 200 characters");
    }
} 