namespace MyHomeRamen.Api.Common.Hateoas.Builder;

public interface IHateoasBuilderFactory
{
    HateoasResponseBuilder<T> ForItem<T>(T item);

    HateoasCollectionResponseBuilder<T> ForCollection<T>(IEnumerable<T> items);
}
