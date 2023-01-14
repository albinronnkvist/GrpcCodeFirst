# Code-first gRPC in .NET

Code-first gRPC using .NET types to define service and message contracts.
It is made possible using the [protobuf-net.Grpc](https://protobuf-net.github.io/protobuf-net.Grpc/) package.

## Validation

Fluent Validation will be added...

## Authentication / Authorization

Azure AD Client Credentials Flow will be added...

## Docker

A [built-in containers](https://learn.microsoft.com/en-us/dotnet/core/docker/publish-as-container) approach is used:

- Build image with dotnet publish: _dotnet publish --os linux --arch x64 -c Release_
- Run container: _docker run -p 5000:80 -d [image-name]:[tag]_
- Push to remote repository: 
  - First change tag: _docker tag [image-name]:[tag] [remote-repository-url]/[image-name]:[new-tag]_
  - Then push: _docker push [remote-repository-url]/[image-name]:[new-tag]_

[_Jammy-chiseled_](https://github.com/dotnet/dotnet-docker/blob/ad733d1665b76ca944213fbce779922c39466a54/src/aspnet/7.0/jammy-chiseled/amd64/Dockerfile) is used as the base image.
It's a small and secure image with only the packages required to run the container, no package manager, no shell and non-root user.

## Testing

It is possible to test the gRPC service using [Postman or gRPCurl](https://learn.microsoft.com/en-us/aspnet/core/grpc/test-tools?view=aspnetcore-7.0).
You could also run tests from the _IntegrationTest_ project. Read more about [testing gRPC services in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/grpc/test-services?view=aspnetcore-7.0).

The reflection service is also enabled, which allows clients to dynamically discover the available methods and their input and output types.
