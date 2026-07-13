using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DZ_MP.CORE.Middlewares;

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // ข้ามการ Log สำหรับ Scalar, OpenAPI และ Swagger เพื่อไม่ให้ JSON ขนาดใหญ่บวมใน Log
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        if (path.StartsWith("/scalar") ||
            path.StartsWith("/openapi") ||
            path.StartsWith("/swagger") ||
            path.EndsWith("favicon.ico"))
        {
            await next(context);
            return;
        }

        var correlationId = context.Items["CorrelationId"]?.ToString()
                            ?? context.TraceIdentifier;

        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            var sw = Stopwatch.StartNew();

            string requestBody;
            if (context.Request.ContentType?.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase) == true)
            {
                requestBody = "(binary/multipart)";
            }
            else
            {
                requestBody = await ReadRequestBodyAsync(context.Request);
            }

            logger.LogInformation("--> HTTP {Method} {Path} | Payload: {RequestBody}",
                context.Request.Method,
                context.Request.Path,
                requestBody);

            var originalBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            try
            {
                await next(context);
            }
            finally
            {
                memStream.Seek(0, SeekOrigin.Begin);

                var contentType = context.Response.ContentType ?? string.Empty;
                var isText = contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase)
                             || contentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase)
                             || contentType.StartsWith("application/problem+json", StringComparison.OrdinalIgnoreCase)
                             || contentType.StartsWith("application/xml", StringComparison.OrdinalIgnoreCase);

                var responseBody = isText
                    ? await new StreamReader(memStream).ReadToEndAsync()
                    : $"(binary {(string.IsNullOrEmpty(contentType) ? "unknown" : contentType)}, {memStream.Length} bytes)";

                sw.Stop();
                logger.LogInformation("<-- HTTP {Method} {Path} responded {StatusCode} in {Elapsed:0.00}ms | Payload: {ResponseBody}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    sw.Elapsed.TotalMilliseconds,
                    responseBody);

                memStream.Seek(0, SeekOrigin.Begin);
                await memStream.CopyToAsync(originalBody);
                context.Response.Body = originalBody;
            }
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength == null || request.ContentLength == 0)
            return "(empty)";

        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Seek(0, SeekOrigin.Begin);
        return body;
    }
}
