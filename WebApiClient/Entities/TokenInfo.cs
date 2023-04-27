using System.Text.Json.Serialization;

namespace Albin.GrpcCodeFirst.WebApiClient.Entities;

public class TokenInfo
{
    public string? TokenType { get; set; }

    public DateTimeOffset ExpiresInUtc { get; set; }

    public string? AccessToken { get; set; }
}
