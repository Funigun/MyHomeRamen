using MyHomeRamen.Api.Common.Hateoas.Common;
using MyHomeRamen.Api.Common.Hateoas.Models;

namespace MyHomeRamen.Api.Common.Hateoas.Builder;

public class HateoasCollectionResponseBuilder<TItem>
{
    private readonly HateoasLinkService _linkService;
    private readonly HateoasCollectionListBuilder<TItem> _collectionListBuilder = default!;

    public ICollection<HateoasResponse<TItem>> Items { get; }

    public ICollection<HateoasLink> Links { get; } = [];

    public HateoasCollectionResponseBuilder(IEnumerable<TItem> items, HateoasLinkService linkService)
    {
        Items = items.Select(item => new HateoasResponse<TItem>(item)).ToList();
        _linkService = linkService;
        _collectionListBuilder = new(Items, Links, linkService);
    }

    protected HateoasCollectionResponseBuilder(IEnumerable<HateoasResponse<TItem>> items, ICollection<HateoasLink> links, HateoasLinkService linkService)
    {
        Items = items.ToList();
        Links = links;
        _linkService = linkService;
    }

    public HateoasCollectionResponseBuilder<TItem> AddGet(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            Links.Add(_linkService.GenerateGet(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasCollectionResponseBuilder<TItem> AddPost(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            Links.Add(_linkService.GeneratePost(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasCollectionResponseBuilder<TItem> AddPut(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            Links.Add(_linkService.GeneratePut(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasCollectionResponseBuilder<TItem> AddDelete(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            Links.Add(_linkService.GenerateDelete(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasCollectionListBuilder<TItem> WithCollectionLink()
    {
        return _collectionListBuilder;
    }

    public HateoasCollectionResponse<TItem> Build()
    {
        return new HateoasCollectionResponse<TItem>(Items) { Links = Links };
    }
}
