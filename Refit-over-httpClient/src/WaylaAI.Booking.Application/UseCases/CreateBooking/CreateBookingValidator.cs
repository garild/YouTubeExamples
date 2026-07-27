using FluentValidation;

namespace WaylaAI.Booking.Application.UseCases.CreateBooking;

public sealed class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Date).GreaterThanOrEqualTo(DateTime.UtcNow.Date);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
