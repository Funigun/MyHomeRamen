using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MyHomeRamen.Blazor.Presentation.Authentication;

public class AuthHeaderHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext ??
            throw new InvalidOperationException("No HttpContext available from the IHttpContextAccessor.");

        string? accessToken = await httpContext.GetTokenAsync("access_token");

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

            System.Security.Claims.ClaimsPrincipal? user = httpContext.User;
            string scheme = "RestaurantCustomer";

            if (user.IsInRole("manager"))
            {
                scheme = "RestaurantManager";
            }
            else if (user.IsInRole("employee"))
            {
                scheme = "RestaurantEmployee";
            }

            request.Headers.Add("x-scheme", scheme);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
