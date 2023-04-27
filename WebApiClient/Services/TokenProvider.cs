using Albin.GrpcCodeFirst.WebApiClient.Entities;
using Albin.GrpcCodeFirst.WebApiClient.Exceptions;
using Albin.GrpcCodeFirst.WebApiClient.Validators;

namespace Albin.GrpcCodeFirst.WebApiClient.Services;

public class TokenProvider : ITokenProvider
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private TokenInfo? _tokenInfo = new();

    public TokenProvider(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task GenerateTokenAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var tokenClient = _serviceProvider.GetRequiredService<ITokenClient>();

            var tokenInfo = await tokenClient.GetAccessTokenAsync();

            _tokenInfo = new TokenInfo
            {
                TokenType = tokenInfo.TokenType,
                ExpiresInUtc = DateTimeOffset.UtcNow.AddSeconds(tokenInfo.ExpiresIn),
                AccessToken = tokenInfo.AccessToken
            };
        }
        catch (TransientTokenException transientTokenException)
        {
            _logger.LogError(transientTokenException, "Transient error occurred when trying to generate an access token: {Message}.", 
                transientTokenException.Message);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Fatal error occurred when trying to generate an access token: {Message}", 
                ex.Message);
        }
    }

    public async Task<string> GetTokenAsync()
    {
        if (_tokenInfo is null)
        {
            await GenerateTokenAsync();
        }
        else
        {
            if (TokenValidator.TokenIsExpired(_tokenInfo.ExpiresInUtc))
            {
                await GenerateTokenAsync();
            }
        }

        ArgumentNullException.ThrowIfNull(_tokenInfo?.AccessToken);

        return _tokenInfo.AccessToken;
    }
}
