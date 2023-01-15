using Albin.GrpcCodeFirst.IntegrationTest.Mocks;
using Albin.GrpcCodeFirst.Server;
using Albin.GrpcCodeFirst.Shared.Contracts;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Client;
using System.Net.Http.Headers;

namespace Albin.GrpcCodeFirst.IntegrationTest.Fixtures;

public sealed class GrpcFixture : IDisposable
{
    private readonly WebApplicationFactory<Startup> _webApplicationFactory;
    private readonly GrpcChannel _grpcChannel;

    public GrpcFixture()
    {
        _webApplicationFactory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Replace auth
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, AuthenticationHandlerMock>(
                            "Test", options => { });
                    services.AddAuthorization(options =>
                    {
                        options.AddPolicy("VIP", policy => policy
                            .AddAuthenticationSchemes("Test")
                            .RequireRole("Club.VipAccess", "Club.Owner")
                            .RequireAuthenticatedUser());
                    });
                });
            });

        var client = _webApplicationFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        _grpcChannel = GrpcChannel.ForAddress(client.BaseAddress!, new GrpcChannelOptions
        {
            HttpClient = client,
            DisposeHttpClient = true
        });
    }

    public async Task<EnterReply> GetEnterClubReplyAsync(EnterRequest request)
    {
        var client = _grpcChannel.CreateGrpcService<IBouncerService>();

        return await client.EnterClubAsync(request);
    }

    public async Task<EnterReply> GetEnterClubVipSectionReplyAsync(EnterRequest request)
    {
        var client = _grpcChannel.CreateGrpcService<IBouncerService>();

        return await client.EnterClubVipSectionAsync(request);
    }

    public void Dispose()
    {
        _webApplicationFactory.Dispose();
        _grpcChannel.Dispose();
    }
}