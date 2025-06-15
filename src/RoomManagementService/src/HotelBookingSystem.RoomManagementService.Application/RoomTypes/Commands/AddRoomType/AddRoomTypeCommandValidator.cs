using FluentValidation;
using HotelBookingSystem.RoomManagementService.Domain.Enums;

namespace HotelBookingSystem.RoomManagementService.Application.RoomTypes.Commands.AddRoomType
{
    public class AddRoomTypeCommandValidator : AbstractValidator<AddRoomTypeCommand>
    {
        public AddRoomTypeCommandValidator()
        {
            RuleFor(v => v.HotelId)
                .NotEmpty()
                .WithMessage("Hotel ID is required.");

            RuleFor(v => v.Name)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Room type name is required and must not exceed 200 characters.");

            RuleFor(v => v.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters.");

            RuleFor(v => v.MaxOccupancy)
                .GreaterThan(0)
                .LessThanOrEqualTo(10)
                .WithMessage("Max occupancy must be between 1 and 10.");

            RuleFor(v => v.BasePrice)
                .GreaterThan(0)
                .WithMessage("Base price must be greater than 0.");

            RuleFor(v => v.Currency)
                .IsInEnum()
                .WithMessage("Valid currency is required.");

            RuleFor(v => v.PrimaryBedType)
                .IsInEnum()
                .NotEqual(BedType.None)
                .WithMessage("Valid primary bed type is required.");

            RuleFor(v => v.PrimaryBedCount)
                .GreaterThan(0)
                .WithMessage("Primary bed count must be greater than 0.");

            RuleFor(v => v.SecondaryBedCount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Secondary bed count must be 0 or greater.");

            RuleFor(v => v.SecondaryBedCount)
                .Equal(0)
                .When(v => v.SecondaryBedType == BedType.None)
                .WithMessage("Secondary bed count must be 0 when secondary bed type is None.");
        }
    }
}