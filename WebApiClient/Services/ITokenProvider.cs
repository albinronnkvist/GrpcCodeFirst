namespace Albin.GrpcCodeFirst.WebApiClient.Services;

public interface ITokenProvider
{
    Task<string> GetTokenAsync();
}
