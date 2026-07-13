using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace DZ_MP.CORE.Middlewares;

/// <summary>
/// A DelegatingHandler responsible for intercepting outgoing HttpClient requests
/// and attaching the current user's JWT access_token to the Authorization header,
/// allowing for seamless token propagation across internal microservices.
/// </summary>
public class AuthTokenPropagationHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;

        if (context != null)
        {
            // 1. Try to get token from Cookie first
            var token = context.Request.Cookies["access_token"];

            // 2. Fallback to Authorization Header if no cookie is found
            if (string.IsNullOrEmpty(token))
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authHeader["Bearer ".Length..].Trim();
                }
            }

            // 3. Attach token and propagate
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
