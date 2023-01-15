using Albin.GrpcCodeFirst.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using ProtoBuf.Grpc;
using System.Data;

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
                    ? $"Everything checks out, welcome in {request.Name}." 
                    : $"You're not old enough to enter the club. Come back in {23 - request.Age} years."
            });
    }

    [Authorize(Policy = "VIP")]
    public Task<EnterReply> EnterClubVipSectionAsync(EnterRequest request, CallContext context = default)
    {
        var isAllowedToEnter = request.Age >= 23;

        return Task.FromResult(
            new EnterReply
            {
                AllowEntry = isAllowedToEnter,
                Message = isAllowedToEnter
                    ? $"Everything checks out, welcome in to the VIP section {request.Name}."
                    : $"You're not old enough to enter the club. Come back in {23 - request.Age} years."
            });
    }
}