namespace MyHomeRamen.Api.Common.Hateoas.Models;

public sealed class HateoasLink
{
    public string Href { get; init; } = default!;

    public string Rel { get; init; } = default!;

    public string Method { get; init; } = default!;

    public HateoasLink()
    {

    }

    private HateoasLink(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }

    public static HateoasLink CreateGet(string href, string rel) =>
        new(href, rel, Hateoas.Common.HateoasMethodConstants.HttpGet);

    public static HateoasLink CreatePost(string href, string rel) =>
        new(href, rel, Hateoas.Common.HateoasMethodConstants.HttpPost);

    public static HateoasLink CreatePut(string href, string rel) =>
        new(href, rel, Hateoas.Common.HateoasMethodConstants.HttpPut);

    public static HateoasLink CreateDelete(string href, string rel) =>
        new(href, rel, Hateoas.Common.HateoasMethodConstants.HttpDelete);
}
