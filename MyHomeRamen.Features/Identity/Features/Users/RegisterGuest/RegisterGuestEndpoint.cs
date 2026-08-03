using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.Identity.Features.Users.RegisterGuest;

public sealed record RegisterGuestRequest(Guid? ExistingGuestId);

public record RegisterGuestResponse(Guid GuestId);

public class RegisterGuestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<RegisterGuestResponse>("api/account/guest", Handler)
                       .WithTags("account")
                       .WithDescription("Creates new guest account or returns existing one")
                       .AllowAnonymous();
    }

    private static async Task<Results<Created<RegisterGuestResponse>, BadRequest>> Handler(
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] ICommandHandler<RegisterGuestCommand, RegisterGuestResponse> handler)
    {
        HttpContext httpContext = httpContextAccessor.HttpContext!;

        Guid? existingGuestId = null;
        if (httpContext.Request.Cookies.TryGetValue("guest_id", out string? guestIdString) && Guid.TryParse(guestIdString, out Guid parsedId))
        {
            existingGuestId = parsedId;
        }

        RegisterGuestCommand command = new(new RegisterGuestRequest(existingGuestId));
        RegisterGuestResponse response = await handler.Handle(command, httpContext.RequestAborted);

        return TypedResults.Created($"/account/guest/{response.GuestId}", response);
    }
}

