using MyHomeRamen.Api.Common.Hateoas.Common;
using MyHomeRamen.Api.Common.Hateoas.Models;

namespace MyHomeRamen.Api.Common.Hateoas.Builder;

public class HateoasCollectionListBuilder<TItem>(IEnumerable<HateoasResponse<TItem>> items, ICollection<HateoasLink> links, HateoasLinkService linkService) : HateoasCollectionResponseBuilder<TItem>(items, links, linkService)
{
    private readonly HateoasLinkService _linkService = linkService;

    public HateoasCollectionListBuilder<TItem> WithGet(LinkOptions options, Func<TItem, object> itemParams)
    {
        if (options.IsActionAllowed)
        {
            foreach (HateoasResponse<TItem> item in Items)
            {
                item.Links.Add(_linkService.GenerateGet(options.Endpoint, itemParams(item.Item), options.Rel));
            }
        }

        return this;
    }

    public HateoasCollectionListBuilder<TItem> WithPost(LinkOptions options, Func<TItem, object> itemParams)
    {
        if (options.IsActionAllowed)
        {
            foreach (HateoasResponse<TItem> item in Items)
            {
                item.Links.Add(_linkService.GeneratePost(options.Endpoint, itemParams(item.Item), options.Rel));
            }
        }

        return this;
    }

    public HateoasCollectionListBuilder<TItem> WithPut(LinkOptions options, Func<TItem, object> itemParams)
    {
        if (options.IsActionAllowed)
        {
            foreach (HateoasResponse<TItem> item in Items)
            {
                item.Links.Add(_linkService.GeneratePut(options.Endpoint, itemParams(item.Item), options.Rel));
            }
        }

        return this;
    }

    public HateoasCollectionListBuilder<TItem> WithDelete(LinkOptions options, Func<TItem, object> itemParams)
    {
        if (options.IsActionAllowed)
        {
            foreach (HateoasResponse<TItem> item in Items)
            {
                item.Links.Add(_linkService.GenerateDelete(options.Endpoint, itemParams(item.Item), options.Rel));
            }
        }

        return this;
    }
}
