namespace Albin.GrpcCodeFirst.WebApiClient.Exceptions;

public class TransientTokenException : Exception
{
    public TransientTokenException()
    { }

    public TransientTokenException(string message) : base(message)
    { }

    public TransientTokenException(string message, Exception inner) : base(message, inner)
    { }
}
