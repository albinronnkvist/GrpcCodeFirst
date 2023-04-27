using Albin.GrpcCodeFirst.WebApiClient.Dtos;

namespace Albin.GrpcCodeFirst.WebApiClient.Services;

public interface IAzureAdClient
{
    Task<TokenInfo> GetAccessTokenAsync();
}
