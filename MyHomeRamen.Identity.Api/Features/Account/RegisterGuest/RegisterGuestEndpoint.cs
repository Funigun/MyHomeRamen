using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Identity.Api.Features.Account.RegisterGuest.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.RegisterGuest;

public class RegisterGuestEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Account";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<RegisterGuestRequest, RegisterGuestResponse>("/guest", Handler)
                       .WithDescription("Creates new guest account or returns existing one")
                       .AllowAnonymous();
    }

    private static async Task<Results<Created<RegisterGuestResponse>, BadRequest>> Handler([FromServices] HttpContext httpContext, [FromServices] RegisterGuestHandler handler)
    {
        Guid? existingGuestId = null;
        if (httpContext.Request.Cookies.TryGetValue("guest_id", out string? guestIdString) && Guid.TryParse(guestIdString, out Guid parsedId))
        {
            existingGuestId = parsedId;
        }

        RegisterGuestRequest request = new(existingGuestId);
        RegisterGuestResponse response = await handler.Handle(request, httpContext.RequestAborted);

        httpContext.Response.Cookies.Append("guest_id", response.GuestId.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Path = "/",
            SameSite = SameSiteMode.Lax
        });

        return TypedResults.Created($"/account/guest/{response.GuestId}", response);
    }
}
