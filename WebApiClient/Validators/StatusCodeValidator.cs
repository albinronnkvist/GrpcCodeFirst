using System.Net;

namespace Albin.GrpcCodeFirst.WebApiClient.Validators;

public static class StatusCodeValidator
{
    /// <summary>
    /// Validates if the HTTP status code is transient
    /// </summary>
    /// <param name="statusCode"></param>
    /// <returns>True, if the HTTP status code is considered to be transient. Otherwise false.</returns>
    public static bool IsTransientHttpStatusCode(HttpStatusCode statusCode)
    {
        return statusCode is HttpStatusCode.RequestTimeout
            or HttpStatusCode.BadGateway
            or HttpStatusCode.ServiceUnavailable
            or HttpStatusCode.GatewayTimeout;
    }
}
