using Albin.GrpcCodeFirst.IntegrationTest.Fixtures;
using Albin.GrpcCodeFirst.Shared.Contracts;

namespace Albin.GrpcCodeFirst.IntegrationTest;

public class EnterClubTests : IClassFixture<GrpcFixture>
{
    public EnterClubTests(GrpcFixture fixture)
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

        var response = await _fixture.GetEnterClubReplyAsync(request);

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

        var response = await _fixture.GetEnterClubReplyAsync(request);

        Assert.False(response.AllowEntry);
    }

    // A valid token is added to the Authorization Header in the fixture.
    // Toggle it on/off or change role to test if it validates correctly.
    [Fact]
    public async Task When23OrOlderTriesToEnterVipSection_AndIsAuthorized_AllowEntry()
    {
        var request = new EnterRequest
        {
            Name = "Albin VIP",
            Age = 25
        };

        var response = await _fixture.GetEnterClubVipSectionReplyAsync(request);

        Assert.True(response.AllowEntry);
    }

    private readonly GrpcFixture _fixture;
}
