using MyHomeRamen.Api.Common.Hateoas.Common;

namespace MyHomeRamen.Api.Common.Hateoas.Builder;

public sealed class HateoasBuilderFactory(HateoasLinkService linkService) : IHateoasBuilderFactory
{
    private HateoasLinkService LinkService { get; } = linkService;

    public HateoasResponseBuilder<TItem> ForItem<TItem>(TItem item) => new(item, LinkService);

    public HateoasCollectionResponseBuilder<TItem> ForCollection<TItem>(IEnumerable<TItem> items) => new(items, LinkService);
}
