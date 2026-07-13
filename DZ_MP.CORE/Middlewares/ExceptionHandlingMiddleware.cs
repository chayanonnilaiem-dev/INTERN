using System.Net;
using DZ_MP.CORE.Commons;
using DZ_MP.CORE.Exceptions;
using DZ_MP.CORE.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DZ_MP.CORE.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BusinessException ex)
        {
            await HandleExceptionAsync(context, ex.StatusCode, ex.ErrorCode, ex.InnerException?.Message ?? ex.Message);
        }
        catch (InternalErrorException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, ex.ErrorCode, ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled Exception occurred.");
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, ErrorCode.INTERNALERROR, ex.InnerException?.Message ?? ex.Message);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string errorCode, string message)
    {
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? context.TraceIdentifier;

        var response = BaseResponse<object>.Failure(statusCode, errorCode, message, correlationId);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsJsonAsync(response);
    }
}
