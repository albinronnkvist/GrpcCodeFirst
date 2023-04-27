using Albin.GrpcCodeFirst.Shared.Contracts;
using Albin.GrpcCodeFirst.WebApiClient.Options;
using Albin.GrpcCodeFirst.WebApiClient.Routes.Version1;
using Albin.GrpcCodeFirst.WebApiClient.Validators;
using FluentValidation;
using Grpc.Core;
using Grpc.Net.Client.Configuration;
using ProtoBuf.Grpc.ClientFactory;
using Albin.GrpcCodeFirst.WebApiClient.Services;
using Polly;
using Polly.Contrib.WaitAndRetry;
using EnterRequest = Albin.GrpcCodeFirst.WebApiClient.Dtos.EnterRequest;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IValidator<EnterRequest>, EnterRequestValidator>();
builder.Services.AddTransient<ITokenClient, TokenClient>();
builder.Services.AddSingleton<ITokenProvider, TokenProvider>();

builder.Services.Configure<BouncerOptions>(builder.Configuration.GetSection(nameof(BouncerOptions)));
builder.Services.Configure<AzureAdOptions>(builder.Configuration.GetSection(nameof(AzureAdOptions)));

var bouncerOptions = builder.Configuration
    .GetSection(nameof(BouncerOptions))
    .Get<BouncerOptions>();
ArgumentNullException.ThrowIfNull(bouncerOptions);

builder.Services.AddCodeFirstGrpcClient<IBouncerService>(opt =>
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

builder.Services.AddHttpClient<ITokenClient, TokenClient>(opt =>
{
    opt.BaseAddress = new Uri("https://login.microsoftonline.com/");
})
.AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)))
.AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(10, TimeSpan.FromSeconds(10)));

var app = builder.Build();

app.MapGroup("/api/v1/bouncer")
    .MapBouncerV1()
    .WithTags("BouncerV1");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
