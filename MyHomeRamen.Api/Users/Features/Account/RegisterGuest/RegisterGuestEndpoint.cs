using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Api.Users.Features.Account.RegisterGuest;

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
