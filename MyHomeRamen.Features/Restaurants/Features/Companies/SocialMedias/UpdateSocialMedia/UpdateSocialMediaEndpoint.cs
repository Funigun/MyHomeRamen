using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.SocialMedias.UpdateSocialMedia;

public sealed class UpdateSocialMediaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPut("api/restaurants/company/social-media/{socialMediaId}", HandleAsync)
                       .WithName("UpdateCompanySocialMedia")
                       .WithDescription("Updates company social media.")
                       .WithTags("restaurants", "company")
                       .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync([FromRoute] Guid socialMediaId, [FromBody] UpdateSocialMediaRequest request, [FromServices] IRequestHandler<UpdateSocialMediaCommand, Unit> handler, CancellationToken cancellationToken)
    {
        await handler.Handle(new UpdateSocialMediaCommand(socialMediaId, request), cancellationToken);
        return Results.NoContent();
    }
}
