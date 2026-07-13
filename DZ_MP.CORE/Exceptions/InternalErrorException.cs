namespace DZ_MP.CORE.Exceptions;

public class InternalErrorException(string errorCode, string message) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
}
