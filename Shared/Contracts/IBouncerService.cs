using System.Runtime.Serialization;
using System.ServiceModel;
using ProtoBuf.Grpc;

namespace Albin.GrpcCodeFirst.Shared.Contracts;

[ServiceContract]
public interface IBouncerService
{
    [OperationContract]
    Task<EnterReply> EnterClubAsync(EnterRequest request, CallContext context = default);
}

[DataContract]
public class EnterReply
{
    [DataMember(Order = 1)]
    public bool AllowEntry { get; set; }

    [DataMember(Order = 2)]
    public required string Message { get; set; }
}

[DataContract]
public class EnterRequest
{
    [DataMember(Order = 1)]
    public required string Name { get; set; }

    [DataMember(Order = 2)]
    public int Age { get; set; }
}