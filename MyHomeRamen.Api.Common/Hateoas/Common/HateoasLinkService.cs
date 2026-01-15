using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Api.Common.Hateoas.Models;

namespace MyHomeRamen.Api.Common.Hateoas.Common;

public sealed class HateoasLinkService(IHttpContextAccessor httpAccessor, LinkGenerator linkGenerator)
{
    public HateoasLink GeneratePost(string endpoint, object? routeValues, string rel)
    {
        return HateoasLink.CreatePost(linkGenerator.GetUriByName(httpAccessor.HttpContext!, endpoint, routeValues)!, rel);
    }

    public HateoasLink GenerateGet(string endpoint, object? routeValues, string rel)
    {
        return HateoasLink.CreateGet(linkGenerator.GetUriByName(httpAccessor.HttpContext!, endpoint, routeValues)!, rel);
    }

    public HateoasLink GeneratePut(string endpoint, object? routeValues, string rel)
    {
        return HateoasLink.CreatePut(linkGenerator.GetUriByName(httpAccessor.HttpContext!, endpoint, routeValues)!, rel);
    }

    public HateoasLink GenerateDelete(string endpoint, object? routeValues, string rel)
    {
        return HateoasLink.CreateDelete(linkGenerator.GetUriByName(httpAccessor.HttpContext!, endpoint, routeValues)!, rel);
    }
}
