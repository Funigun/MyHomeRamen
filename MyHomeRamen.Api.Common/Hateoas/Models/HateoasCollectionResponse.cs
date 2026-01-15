namespace MyHomeRamen.Api.Common.Hateoas.Models;

public class HateoasCollectionResponse<TItem>
{
    public ICollection<HateoasResponse<TItem>> Items { get; init; }

    public ICollection<HateoasLink> Links { get; init; } = [];

    public HateoasCollectionResponse()
    {
        Items = [];
    }

    public HateoasCollectionResponse(ICollection<HateoasResponse<TItem>> items)
    {
        Items = items;
    }
}
