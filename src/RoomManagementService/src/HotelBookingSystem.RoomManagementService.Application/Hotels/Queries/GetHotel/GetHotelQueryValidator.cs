using FluentValidation;

namespace HotelBookingSystem.RoomManagementService.Application.Hotels.Queries.GetHotel
{
    public class GetHotelQueryValidator : AbstractValidator<GetHotelQuery>
    {
        public GetHotelQueryValidator()
        {
            RuleFor(v => v.HotelId)
                .NotEmpty()
                .WithMessage("Hotel ID is required.");
        }
    }
}