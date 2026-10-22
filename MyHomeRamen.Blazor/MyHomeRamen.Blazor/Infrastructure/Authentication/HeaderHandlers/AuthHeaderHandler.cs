using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MyHomeRamen.Blazor.Infrastructure.Authentication.HeaderHandlers;

public class AuthHeaderHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            string? accessToken = await httpContext.GetTokenAsync("access_token");

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
