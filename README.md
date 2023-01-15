# Code-first gRPC in .NET

Code-first gRPC using .NET types to define service and message contracts.
It is made possible using the [protobuf-net.Grpc](https://protobuf-net.github.io/protobuf-net.Grpc/) package.

## Validation

Request message validation is done with the [grpc-aspnetcore-validator](https://github.com/AnthonyGiretti/grpc-aspnetcore-validator) package. It is integrated with [Fluent Validation](https://github.com/FluentValidation/FluentValidation).

To list validation errors on the client, see the following example:

```
try
{
    using var channel = GrpcChannel.ForAddress("https://localhost:7039");
    var client =  channel.CreateGrpcService<IBouncerService>();
    
    // Empty Name value and incorrect Age value that raises validation errors
    var reply = await client.EnterClubAsync(new EnterRequest { Name = "", Age = -25 });
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
{
    var errors = ex.GetValidationErrors(); // Gets list of validation errors
}
```

## Authentication / Authorization

[Azure AD Client Credentials Flow](https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-client-creds-grant-flow) is used.

See the [Azure AD Client Credentials Flow with ASP.NET Core Web API](https://medium.com/@albinronnkvist/azure-ad-client-credential-flow-with-asp-net-core-web-api-f2a1f7b29532) article for a full tutorial on how to implement it.

## Testing

It is possible to test the gRPC service using [Postman or gRPCurl](https://learn.microsoft.com/en-us/aspnet/core/grpc/test-tools?view=aspnetcore-7.0).

You can also run tests from the _IntegrationTest_ project. Read more about [testing gRPC services in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/grpc/test-services?view=aspnetcore-7.0).

The reflection service is also enabled, which allows clients to dynamically discover the available methods and their input and output types.

## Docker

A [built-in containers](https://learn.microsoft.com/en-us/dotnet/core/docker/publish-as-container) approach is used:

- Build image with dotnet publish: _dotnet publish --os linux --arch x64 -c Release_
- Run container: _docker run -p 5000:80 -d [image-name]:[tag]_
- Push to remote repository: 
  - First change tag: _docker tag [image-name]:[tag] [remote-repository-url]/[image-name]:[new-tag]_
  - Then push: _docker push [remote-repository-url]/[image-name]:[new-tag]_

[_Jammy-chiseled_](https://github.com/dotnet/dotnet-docker/blob/ad733d1665b76ca944213fbce779922c39466a54/src/aspnet/7.0/jammy-chiseled/amd64/Dockerfile) is used as the base image.
It's a small and secure image with only the packages required to run the container, no package manager, no shell and non-root user.
