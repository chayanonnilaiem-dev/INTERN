using System.Net;

namespace DZ_MP.CORE.Models.DTO;

public class BaseResponse<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public T? Data { get; set; }
    public string? TraceId { get; set; }
    public List<FieldErrorDetail>? Errors { get; set; }

    public static BaseResponse<T> Ok(T? data = default, string? message = null)
    {
        return new BaseResponse<T>
        {
            StatusCode = HttpStatusCode.OK,
            Data = data,
            Message = message
        };
    }

    public static BaseResponse<T> Failure(
        HttpStatusCode statusCode,
        string errorCode,
        string message,
        string? traceId = null,
        List<FieldErrorDetail>? errors = null)
    {
        return new BaseResponse<T>
        {
            StatusCode = statusCode,
            ErrorCode = errorCode,
            Message = message,
            TraceId = traceId,
            Errors = errors
        };
    }
}

public record FieldErrorDetail(string Field, string Message);
