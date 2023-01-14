using Albin.GrpcCodeFirst.Shared.Contracts;
using ProtoBuf.Grpc;

namespace Albin.GrpcCodeFirst.Server.Services;

public class BouncerService : IBouncerService
{
    public Task<EnterReply> EnterClubAsync(EnterRequest request, CallContext context = default)
    {
        var isAllowedToEnter = request.Age >= 23;

        return Task.FromResult(
            new EnterReply
            {
                AllowEntry = isAllowedToEnter,
                Message = isAllowedToEnter 
                    ? $"Welcome {request.Name}" 
                    : $"You're not old enough to enter the club. Come back in {23 - request.Age} years."
            });
    }
}