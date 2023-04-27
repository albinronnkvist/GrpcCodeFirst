using Albin.GrpcCodeFirst.Shared.Contracts;
using FluentValidation;
using EnterRequest = Albin.GrpcCodeFirst.WebApiClient.Dtos.EnterRequest;

namespace Albin.GrpcCodeFirst.WebApiClient.Routes.Version1;

public static class Bouncer
{
    public static RouteGroupBuilder MapBouncerV1(this RouteGroupBuilder group)
    {
        group.MapPost("/authorize-entry", AuthorizeEntry);

        return group;
    }

    public static async Task<IResult> AuthorizeEntry(IValidator<EnterRequest> validator, IBouncerService bouncerService, EnterRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var grpcRequest = new Shared.Contracts.EnterRequest
        {
            Name = request.Name,
            Age = request.Age
        };

        var decision = await bouncerService.EnterClubAsync(grpcRequest);
        return TypedResults.Ok(decision);
    }
}
