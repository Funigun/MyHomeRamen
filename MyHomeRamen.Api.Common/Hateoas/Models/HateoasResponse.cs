namespace MyHomeRamen.Api.Common.Hateoas.Models;

public sealed class HateoasResponse<TItem>
{
    public TItem Item { get; init; } = default!;

    public ICollection<HateoasLink> Links { get; } = [];

    public HateoasResponse()
    {

    }

    public HateoasResponse(TItem value)
    {
        Item = value;
    }

    public HateoasResponse(TItem value, ICollection<HateoasLink> links) : this(value)
    {
        Links = links;
    }
}
