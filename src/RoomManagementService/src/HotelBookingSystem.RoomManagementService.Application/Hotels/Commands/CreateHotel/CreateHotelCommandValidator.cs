using FluentValidation;

namespace HotelBookingSystem.RoomManagementService.Application.Hotels.Commands.CreateHotel
{
    public class CreateHotelCommandValidator : AbstractValidator<CreateHotelCommand>
    {
        public CreateHotelCommandValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Hotel name is required and must not exceed 200 characters.");

            RuleFor(c => c.Description)
                .NotEmpty()
                .MaximumLength(1000)
                .WithMessage("Hotel description is required and must not exceed 1000 characters.");

            RuleFor(c => c.ContactEmail)
                .EmailAddress()
                .When(c => !string.IsNullOrEmpty(c.ContactEmail))
                .WithMessage("Valid email address is required.");

            RuleFor(c => c.ContactPhone)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .When(c => !string.IsNullOrEmpty(c.ContactPhone))
                .WithMessage("Valid phone number is required.");

            RuleFor(v => v)
            .Must(HaveAtLeastOneContactMethod)
            .WithMessage("At least one contact method (email or phone) must be provided.");

            RuleFor(v => v.Address)
                .NotNull()
                .WithMessage("Address is required.")
                .SetValidator(new CreateAddressDtoValidator());
        }

        private static bool HaveAtLeastOneContactMethod(CreateHotelCommand command)
        {
            return !string.IsNullOrWhiteSpace(command.ContactEmail) ||
                   !string.IsNullOrWhiteSpace(command.ContactPhone);
        }
    }

    public class CreateAddressDtoValidator : AbstractValidator<CreateAddressDto>
    {
        public CreateAddressDtoValidator()
        {
            RuleFor(v => v.Street)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Street address is required and must not exceed 200 characters.");

            RuleFor(v => v.City)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("City is required and must not exceed 100 characters.");

            RuleFor(v => v.State)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("State is required and must not exceed 100 characters.");

            RuleFor(v => v.Country)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Country is required and must not exceed 100 characters.");

            RuleFor(v => v.PostalCode)
                .NotEmpty()
                .MaximumLength(20)
                .WithMessage("Postal code is required and must not exceed 20 characters.");
        }
    }
}