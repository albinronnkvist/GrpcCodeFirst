using Albin.GrpcCodeFirst.WebApiClient.Dtos;

namespace Albin.GrpcCodeFirst.WebApiClient.Services;

public interface ITokenClient
{
    Task<TokenInfo> GetAccessTokenAsync();
}
