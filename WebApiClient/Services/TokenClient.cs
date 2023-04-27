using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Albin.GrpcCodeFirst.WebApiClient.Dtos;
using Albin.GrpcCodeFirst.WebApiClient.Exceptions;
using Albin.GrpcCodeFirst.WebApiClient.Options;
using Albin.GrpcCodeFirst.WebApiClient.Validators;
using Microsoft.Extensions.Options;

namespace Albin.GrpcCodeFirst.WebApiClient.Services;

public class TokenClient : ITokenClient
{
    private readonly HttpClient _httpClient;
    private readonly AzureAdOptions _options;

    public TokenClient(HttpClient httpClient, IOptions<AzureAdOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<TokenInfo> GetAccessTokenAsync()
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        var endpoint = $"{_options.DirectoryId}/oauth2/v2.0/token";

        var request = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", _options.ClientApplicationId),
            new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
            new KeyValuePair<string, string>("scope", $"{_options.ClientApplicationId}./default"),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        var response = await _httpClient.PostAsync(endpoint, request);
        if (!response.IsSuccessStatusCode)
        {
            await HandleFailure(response, endpoint);
        }

        var result = await response.Content.ReadAsStringAsync();
        var tokenInfo = JsonSerializer.Deserialize<TokenInfo>(result);
        ArgumentNullException.ThrowIfNull(tokenInfo);

        return tokenInfo;
    }

    private async Task HandleFailure(HttpResponseMessage response, string endpoint)
    {
        var request = $"HTTP {response.RequestMessage?.Method.Method} {_httpClient.BaseAddress}{endpoint}";
        var responseContent = await response.Content.ReadAsStringAsync();

        if (StatusCodeValidator.IsTransientHttpStatusCode(response.StatusCode))
        {
            if (response.StatusCode == HttpStatusCode.RequestTimeout)
            {
                throw new TokenException($"Http-Timeout: {request} request timed out, configured timeout is {_httpClient.Timeout}");
            }

            throw new TokenException($"Http-{response.StatusCode}: " +
                                     $"{request} request failed due to a transient error.");
        }

        throw new TokenException($"Http-{response.StatusCode}: " +
                                 $"{request} request failed. " +
                                 $"Content: {responseContent}.");
    }
}
