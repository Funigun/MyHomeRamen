using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.SocialMedias.GetSocialMediaForManage;

public sealed class GetSocialMediaForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetSocialMediaForManageResponse>("api/restaurants/company/social-media/manage", HandleAsync)
                       .WithName("GetCompanySocialMediaForManage")
                       .WithDescription("Returns company social media for management.")
                       .WithTags("restaurants", "company")
                       .RequireAuthorization();
    }

    private static async Task<Results<Ok<GetSocialMediaForManageResponse>, ForbidHttpResult>> HandleAsync([FromServices] IRequestHandler<GetSocialMediaForManageQuery, GetSocialMediaForManageResponse> handler, CancellationToken cancellationToken)
    {
        GetSocialMediaForManageResponse response = await handler.Handle(new GetSocialMediaForManageQuery(), cancellationToken);
        
        return TypedResults.Ok(response);
     }
}
