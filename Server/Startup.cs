using Albin.GrpcCodeFirst.Server.Services;
using Calzolari.Grpc.AspNetCore.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using ProtoBuf.Grpc.Server;

namespace Albin.GrpcCodeFirst.Server;

public class Startup
{
    public Startup(IConfiguration config)
    {
        _config = config;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCodeFirstGrpc(options =>
        {
            if (DetailedErrorsEnabled)
            {
                options.EnableDetailedErrors = true;
            }

            options.EnableMessageValidation();
        });
        services.AddValidators();
        services.AddGrpcValidation();

        services.AddCodeFirstGrpcReflection();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(_config.GetSection("AzureAd"));

        services.AddAuthorization(options =>
        {
            options.AddPolicy("VIP", policy => policy
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireRole("Club.VipAccess, Club.Owner")
                .RequireAuthenticatedUser());
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<BouncerService>();
            endpoints.MapCodeFirstGrpcReflectionService();
            endpoints.MapGet("/",
                () => "Communication with gRPC endpoints must be made through a gRPC client. " +
                      "To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
        });

    }

    private bool DetailedErrorsEnabled => bool.Parse(_config["EnableDetailedErrors"] ?? bool.FalseString);
    private readonly IConfiguration _config;
}