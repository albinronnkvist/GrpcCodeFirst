using Albin.GrpcCodeFirst.WebApiClient.Dtos;
using FluentValidation;

namespace Albin.GrpcCodeFirst.WebApiClient.Validators;

public class EnterRequestValidator : AbstractValidator<EnterRequest>
{
    public EnterRequestValidator()
    {
        RuleFor(guest => guest.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

        RuleFor(guest => guest.Age)
            .NotEmpty().WithMessage("Age is required.");
    }
}
