using System.Net;

namespace DZ_MP.CORE.Exceptions;

public class BusinessException(HttpStatusCode statusCode, string errorCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string ErrorCode { get; } = errorCode;
}
