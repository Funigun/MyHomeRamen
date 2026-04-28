# Plan: Register Guest — Frontend

## Metadata

**Type:** Feature  
**Layers Affected:** Blazor (Server)  
**Created:** 2025-01-29

---

## References

- Existing typed HTTP client: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Account/Common/Services/CustomerAccountApiClient.cs`
- HTTP client DI registration: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Presentation/ApiDependencyInjection.cs`
- Auth handler used for unauthenticated / anonymous calls: the Identity API endpoint is `.AllowAnonymous()` so **no auth header handler** should be added — register the client without a delegating handler (or with a handler that passes cookies through)
- App entry point / layout: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Components/App.razor`
- Blazor Server render mode: `InteractiveServerRenderMode(prerender: false)` — `HttpContext` is available in the server render pipeline

---

## Implementation Plan

### Step 1: Create frontend feature structure

No new page or component required. This is a transparent background registration — no UI changes.

The feature lives entirely in:
```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Account/
└── Common/
    ├── Models/
    │   └── RegisterGuestResponse.cs      ← NEW — API response DTO
    └── Services/
        └── CustomerAccountApiClient.cs   ← MODIFY — add RegisterGuestAsync method
```

---

### Step 2: Create API Response Model

File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Account/Common/Models/RegisterGuestResponse.cs`

```csharp
namespace MyHomeRamen.Blazor.Features.Account.Common.Models;

public record RegisterGuestResponse(Guid GuestId);
```

---

### Step 3: Update `CustomerAccountApiClient`

File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Account/Common/Services/CustomerAccountApiClient.cs`

Add method `RegisterGuestAsync`:

```csharp
public async Task<RegisterGuestResponse?> RegisterGuestAsync(CancellationToken ct = default)
{
    using HttpResponseMessage response = await httpClient.PostAsync("/api/account/guest", null, ct);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadFromJsonAsync<RegisterGuestResponse>(cancellationToken: ct);
}
```

> **Cookie forwarding note:** The `HttpClient` used by `CustomerAccountApiClient` targets the Identity API from the Blazor Server side. When the Identity API sets `Set-Cookie: guest_id=...` in its response, the Blazor Server must forward that `Set-Cookie` header back to the browser. This is handled in the middleware/service calling `RegisterGuestAsync` (see Step 4).

---

### Step 4: Create `GuestSessionService`

File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Account/Common/Services/GuestSessionService.cs`

This service is responsible for:
1. Checking if the incoming HTTP request already carries a valid `guest_id` cookie.
2. If not, calling `CustomerAccountApiClient.RegisterGuestAsync(...)`.
3. Forwarding the `Set-Cookie` header from the Identity API response back to the browser response via `IHttpContextAccessor`.

```csharp
// Pseudocode
public class GuestSessionService(CustomerAccountApiClient accountApiClient, IHttpContextAccessor httpContextAccessor)
{
    public async Task EnsureGuestSessionAsync(CancellationToken cancellationToken = default)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null) return;

        // Already has a guest cookie — no action needed
        if (httpContext.Request.Cookies.ContainsKey("guest_id")) return;

        // Authenticated users do not need a guest session
        if (httpContext.User.Identity?.IsAuthenticated == true) return;

        RegisterGuestResponse? response = await accountApiClient.RegisterGuestAsync(cancellationToken);

        if (response is not null)
        {
            // The Identity API already set the HttpOnly cookie in its own response.
            // We mirror it here on the Blazor Server response so the browser receives it.
            httpContext.Response.Cookies.Append("guest_id", response.GuestId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                Path = "/",
                SameSite = SameSiteMode.Lax
            });
        }
    }
}
```

> **Why set the cookie from Blazor Server and not rely on Identity API's Set-Cookie?**  
> `HttpClient` calls between services (Blazor Server → Identity API) do not automatically forward `Set-Cookie` response headers to the end browser. The Blazor Server must explicitly append the cookie to its own outbound response. The `GuestId` value is retrieved from the JSON response body, making this safe and explicit.

---

### Step 5: Register `GuestSessionService` in DI

File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Presentation/ApiDependencyInjection.cs`  
(or a new `GuestDependencyInjection.cs` if preferred — follow existing registration file conventions)

- Register `GuestSessionService` as `scoped`:
  ```csharp
  services.AddScoped<GuestSessionService>();
  ```

> `CustomerAccountApiClient` is already registered as a typed `HttpClient` (scoped by default for `IHttpClientFactory`) so it can be injected into `GuestSessionService`.

---

### Step 6: Invoke `GuestSessionService` on First Anonymous Load

The guest session initialization must run on the server side **before** the page is rendered, so the cookie is set in the HTTP response.

**Option A — Middleware (recommended):**

File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Presentation/GuestSessionMiddleware.cs` (NEW)

```csharp
public class GuestSessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, GuestSessionService guestSessionService)
    {
        await guestSessionService.EnsureGuestSessionAsync(context.RequestAborted);
        await next(context);
    }
}
```

Register in `Program.cs` before `app.MapRazorComponents`:
```csharp
app.UseMiddleware<GuestSessionMiddleware>();
```

**Why middleware over a Blazor component?**  
Blazor Server components run after the HTTP response headers may already be partially flushed. Middleware runs in the HTTP pipeline, ensuring `Set-Cookie` headers are written before response body starts. Since `prerender: false` is used, headers are still writable at middleware stage.

---

### Step 7: Ensure `guest_id` Cookie Is Forwarded to Identity API on Subsequent Requests

File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Presentation/Authentication/AuthHeaderHandler.cs`  
(or create a new `GuestCookieForwardingHandler.cs` — preferred to keep concerns separate)

File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Presentation/Authentication/GuestCookieForwardingHandler.cs` (NEW)

This `DelegatingHandler` reads the `guest_id` cookie from the incoming browser request (via `IHttpContextAccessor`) and forwards it in the `Cookie` header of outbound `HttpClient` requests to the Identity API:

```csharp
public class GuestCookieForwardingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;

        if (httpContext is not null &&
            httpContext.Request.Cookies.TryGetValue("guest_id", out string? guestId) &&
            !string.IsNullOrWhiteSpace(guestId))
        {
            request.Headers.TryAddWithoutValidation("Cookie", $"guest_id={guestId}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
```

Register the handler in DI and attach it to `CustomerAccountApiClient` registration in `ApiDependencyInjection.cs`:

```csharp
services.AddTransient<GuestCookieForwardingHandler>();

services.AddHttpClient<CustomerAccountApiClient>(client =>
    {
        client.BaseAddress = new Uri($"https+http://{ServiceNames.IdentityApi(infrastructurePrefix)}");
    }
).AddHttpMessageHandler<AuthHeaderHandler>()
 .AddHttpMessageHandler<GuestCookieForwardingHandler>();
```

> This allows the Identity API backend to perform the idempotency check using the `guest_id` cookie on subsequent calls.

