using Microsoft.AspNetCore.Http;

namespace DZ_MP.CORE.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        var correlationIdStr = correlationId.ToString();
        context.Items["CorrelationId"] = correlationIdStr;
        context.Response.Headers.Append(CorrelationIdHeader, correlationId);
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationIdHeader))
                context.Response.Headers[CorrelationIdHeader] = correlationIdStr;
            return Task.CompletedTask;
        });
        await next(context);
    }
}

public class CorrelationIdHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString()
                            ?? Guid.NewGuid().ToString();

        request.Headers.TryAddWithoutValidation("X-Correlation-Id", correlationId);

        return await base.SendAsync(request, cancellationToken);
    }
}
