using Grpc.Core;
using Grpc.Net.Client.Configuration;

namespace Albin.GrpcCodeFirst.Client;

internal static class RetryPolicyConfig
{
    /// <summary>
    /// <para>This method is configured with MethodName.Default,
    /// so it's applied to all gRPC methods called by the channel that uses this config.</para>
    /// <para>Calls are retried when the previous number of attempts is less than MaxAttempts.</para>
    /// <para>Calls are retried when the failing status code matches a value in RetryableStatusCodes.</para>
    /// <para>Also calls are only retried when the deadline hasn't been exceeded or the call hasn't been committed.</para>
    /// </summary>
    /// <returns></returns>
    internal static MethodConfig GetBaseRetryPolicyConfig()
    {
        return new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable }
            }
        };
    }
}
