using FluentValidation;

namespace HotelBookingSystem.InventoryService.Application.Commands.HoldRoom;

public class HoldRoomCommandValidator : AbstractValidator<HoldRoomCommand>
{
    public HoldRoomCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required");

        RuleFor(x => x.HotelId)
            .NotEmpty()
            .WithMessage("Hotel ID is required");

        RuleFor(x => x.RoomTypeId)
            .NotEmpty()
            .WithMessage("Room Type ID is required");

        RuleFor(x => x.CheckIn)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Check-in date cannot be in the past");

        RuleFor(x => x.CheckOut)
            .GreaterThan(x => x.CheckIn)
            .WithMessage("Check-out date must be after check-in date");

        RuleFor(x => x.RoomCount)
            .GreaterThan(0)
            .WithMessage("Room count must be greater than zero")
            .LessThanOrEqualTo(50)
            .WithMessage("Room count cannot exceed 50");

        RuleFor(x => x.HoldDuration)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Hold duration must be positive")
            .LessThanOrEqualTo(TimeSpan.FromHours(24))
            .WithMessage("Hold duration cannot exceed 24 hours");
    }
} 