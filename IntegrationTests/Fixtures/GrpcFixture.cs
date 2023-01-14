using Albin.GrpcCodeFirst.Server;
using Albin.GrpcCodeFirst.Shared.Contracts;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using ProtoBuf.Grpc.Client;

namespace Albin.GrpcCodeFirst.IntegrationTest.Fixtures;

public sealed class GrpcFixture : IDisposable
{
    private readonly WebApplicationFactory<Startup> _webApplicationFactory;
    private readonly GrpcChannel _grpcChannel;

    public GrpcFixture()
    {
        _webApplicationFactory = new WebApplicationFactory<Startup>();

        var client = _webApplicationFactory.CreateClient();
        _grpcChannel = GrpcChannel.ForAddress(client.BaseAddress!, new GrpcChannelOptions
        {
            HttpClient = client,
            DisposeHttpClient = true
        });
    }

    public async Task<EnterReply> GetEnterReplyAsync(EnterRequest request)
    {
        var client = _grpcChannel.CreateGrpcService<IBouncerService>();

        return await client.EnterClubAsync(request);
    }

    public void Dispose()
    {
        _webApplicationFactory.Dispose();
        _grpcChannel.Dispose();
    }
}