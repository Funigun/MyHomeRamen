using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.SocialMedias.DeleteSocialMedia;

public sealed class DeleteSocialMediaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete("api/restaurants/company/social-media/{socialMediaId}", HandleAsync)
                       .WithName("DeleteCompanySocialMedia")
                       .WithDescription("Deletes company social media.")
                       .WithTags("restaurants", "company")
                       .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync([FromRoute] Guid socialMediaId, [FromServices] IRequestHandler<DeleteSocialMediaCommand, Unit> handler, CancellationToken cancellationToken)
    {
        await handler.Handle(new DeleteSocialMediaCommand(socialMediaId), cancellationToken);
        return Results.NoContent();
    }
}
