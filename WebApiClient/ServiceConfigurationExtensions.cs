using Albin.GrpcCodeFirst.Shared.Contracts;
using Albin.GrpcCodeFirst.WebApiClient.Options;
using Albin.GrpcCodeFirst.WebApiClient.Services;
using Albin.GrpcCodeFirst.WebApiClient.Validators;
using FluentValidation;
using Grpc.Core;
using Grpc.Net.Client.Configuration;
using Polly;
using Polly.Contrib.WaitAndRetry;
using ProtoBuf.Grpc.ClientFactory;
using EnterRequest = Albin.GrpcCodeFirst.WebApiClient.Dtos.EnterRequest;

namespace Albin.GrpcCodeFirst.WebApiClient;

public static class ServiceConfigurationExtensions
{
    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BouncerOptions>(configuration.GetSection(nameof(BouncerOptions)));
        services.Configure<AzureAdOptions>(configuration.GetSection(nameof(AzureAdOptions)));
    }

    public static void ConfigureValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<EnterRequest>, EnterRequestValidator>();
    }

    public static void ConfigureGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        var bouncerOptions = configuration
            .GetSection(nameof(BouncerOptions))
            .Get<BouncerOptions>();
        ArgumentNullException.ThrowIfNull(bouncerOptions);

        services.AddCodeFirstGrpcClient<IBouncerService>(opt =>
            {
                opt.Address = new Uri(bouncerOptions.BaseUrl);
                opt.ChannelOptionsActions.Add(options =>
                {
                    options.HttpHandler = new SocketsHttpHandler
                    {
                        PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                        KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                        KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                        EnableMultipleHttp2Connections = true
                    };
                    options.ServiceConfig = new ServiceConfig
                    {
                        MethodConfigs = {
                            new MethodConfig
                            {
                                Names = { MethodName.Default },
                                RetryPolicy = new RetryPolicy
                                {
                                    MaxAttempts = 3,
                                    InitialBackoff = TimeSpan.FromSeconds(1),
                                    MaxBackoff = TimeSpan.FromSeconds(5),
                                    BackoffMultiplier = 1.5,
                                    RetryableStatusCodes = { StatusCode.Unavailable }
                                }
                            }
                        }
                    };
                });
            })
            .AddCallCredentials(async (context, metadata, serviceProvider) =>
            {
                var provider = serviceProvider.GetRequiredService<ITokenProvider>();
                var token = await provider.GetTokenAsync();

                metadata.Add("Authorization", $"Bearer {token}");
            })
            .EnableCallContextPropagation();
    }

    public static void ConfigureHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<ITokenClient, TokenClient>(opt =>
            {
                opt.BaseAddress = new Uri("https://login.microsoftonline.com/");
            })
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(10, TimeSpan.FromSeconds(10)));
    }

    public static void ConfigureInternalServices(this IServiceCollection services)
    {
        services.AddTransient<ITokenClient, TokenClient>();
        // TODO: use decorator instead of singleton, and use caching + AES encryption to store access token... Scrutor etc...
        services.AddSingleton<ITokenProvider, TokenProvider>();
    }
}
