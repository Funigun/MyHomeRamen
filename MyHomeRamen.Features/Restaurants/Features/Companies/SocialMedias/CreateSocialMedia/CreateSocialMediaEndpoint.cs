using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.SocialMedias.CreateSocialMedia;

public sealed class CreateSocialMediaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<Unit>("api/restaurants/company/social-media", HandleAsync)
                       .WithName("CreateCompanySocialMedia")
                       .WithDescription("Adds social media to the company.")
                       .WithTags("restaurants", "company")
                       .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync([FromBody] CreateSocialMediaRequest request, [FromServices] IRequestHandler<CreateSocialMediaCommand, Unit> handler, CancellationToken cancellationToken)
    {
        await handler.Handle(new CreateSocialMediaCommand(request), cancellationToken);
        return Results.StatusCode(StatusCodes.Status201Created);
    }
}
