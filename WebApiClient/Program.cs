using Albin.GrpcCodeFirst.WebApiClient;
using Albin.GrpcCodeFirst.WebApiClient.Routes.Version1;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOptions(builder.Configuration);
builder.Services.ConfigureGrpcClients(builder.Configuration);
builder.Services.ConfigureHttpClients();
builder.Services.ConfigureValidators();
builder.Services.ConfigureInternalServices();

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
