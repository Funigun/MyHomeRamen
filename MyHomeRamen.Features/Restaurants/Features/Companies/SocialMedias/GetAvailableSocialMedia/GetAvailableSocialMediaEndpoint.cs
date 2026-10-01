using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.SocialMedias.GetAvailableSocialMedia;

public sealed class GetAvailableSocialMediaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetAvailableSocialMediaResponse>("api/restaurants/company/social-media/available", HandleAsync)
                       .WithName("GetAvailableCompanySocialMedia")
                       .WithDescription("Returns public company social media links.")
                       .WithTags("restaurants", "company")
                       .AllowAnonymous();
    }

    private static async Task<Results<Ok<GetAvailableSocialMediaResponse>, ForbidHttpResult>> HandleAsync([FromServices] IRequestHandler<GetAvailableSocialMediaQuery, GetAvailableSocialMediaResponse> handler, CancellationToken cancellationToken)
    {
        GetAvailableSocialMediaResponse response = await handler.Handle(new(), cancellationToken);

        return TypedResults.Ok(response);
    }
}
