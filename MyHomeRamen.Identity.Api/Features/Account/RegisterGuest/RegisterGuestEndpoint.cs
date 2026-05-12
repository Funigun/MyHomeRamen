using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Identity.Api.Features.Account.RegisterGuest.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.RegisterGuest;

public class RegisterGuestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<RegisterGuestRequest, RegisterGuestResponse>("api/account/guest", Handler)
                       .WithTags("account")
                       .WithDescription("Creates new guest account or returns existing one")
                       .AllowAnonymous();
    }

    private static async Task<Results<Created<RegisterGuestResponse>, BadRequest>> Handler([FromServices] IHttpContextAccessor httpContextAccessor, [FromServices] IRequestHandler<RegisterGuestRequest, RegisterGuestResponse> handler)
    {
        HttpContext httpContext = httpContextAccessor.HttpContext!;

        Guid? existingGuestId = null;
        if (httpContext.Request.Cookies.TryGetValue("guest_id", out string? guestIdString) && Guid.TryParse(guestIdString, out Guid parsedId))
        {
            existingGuestId = parsedId;
        }

        RegisterGuestRequest request = new(existingGuestId);
        RegisterGuestResponse response = await handler.Handle(request, httpContext.RequestAborted);

        return TypedResults.Created($"/account/guest/{response.GuestId}", response);
    }
}
