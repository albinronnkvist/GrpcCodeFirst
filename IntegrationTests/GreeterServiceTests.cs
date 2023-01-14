using Albin.GrpcCodeFirst.IntegrationTest.Fixtures;
using Albin.GrpcCodeFirst.Shared.Contracts;

namespace Albin.GrpcCodeFirst.IntegrationTest;

public class GreeterServiceTests : IClassFixture<GrpcFixture>
{
    public GreeterServiceTests(GrpcFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task When23OrOlderTriesToEnter_AllowEntry()
    {
        var request = new EnterRequest
        {
            Name = "Albin",
            Age = 25
        };

        var response = await _fixture.GetEnterReplyAsync(request);

        Assert.True(response.AllowEntry);
    }

    [Fact]
    public async Task WhenYoungerThan23TriesToEnter_DenyEntry()
    {
        var request = new EnterRequest
        {
            Name = "Albin",
            Age = 22
        };

        var response = await _fixture.GetEnterReplyAsync(request);

        Assert.False(response.AllowEntry);
    }

    private readonly GrpcFixture _fixture;
}
