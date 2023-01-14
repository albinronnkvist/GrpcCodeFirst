using Albin.GrpcCodeFirst.Shared.Contracts;
using FluentValidation;

namespace Albin.GrpcCodeFirst.Server.Validators;

public class EnterRequestValidator : AbstractValidator<EnterRequest>
{
    public EnterRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Age).GreaterThan(0);
    }
}
