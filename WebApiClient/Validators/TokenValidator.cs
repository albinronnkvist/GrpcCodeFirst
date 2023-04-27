using Albin.GrpcCodeFirst.WebApiClient.Entities;

namespace Albin.GrpcCodeFirst.WebApiClient.Validators;

public static class TokenValidator
{
    public static bool TokenIsExpired(DateTimeOffset tokenExpiresInUtc)
    {
        var isNotExpiredIfGreaterThanZero = DateTimeOffset.Compare(tokenExpiresInUtc, DateTimeOffset.UtcNow);
        return isNotExpiredIfGreaterThanZero <= 0;
    }
}
