using System.Runtime.Serialization;
using System.ServiceModel;
using ProtoBuf.Grpc;

namespace Albin.GrpcCodeFirst.Shared.Contracts;

[ServiceContract]
public interface IBouncerService
{
    [OperationContract]
    Task<EnterReply> EnterClubAsync(EnterRequest request, CallContext context = default);

    [OperationContract]
    Task<EnterReply> EnterClubVipSectionAsync(EnterRequest request, CallContext context = default);
}

[DataContract]
public record EnterReply
{
    [DataMember(Order = 1)]
    public bool AllowEntry { get; init; }

    [DataMember(Order = 2)]
    public required string Message { get; init; }
}

[DataContract]
public record EnterRequest
{
    [DataMember(Order = 1)]
    public required string Name { get; init; }

    [DataMember(Order = 2)]
    public int Age { get; init; }
}