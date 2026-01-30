namespace MyHomeRamen.Api.Common.Endpoint;

public interface IRequestId
{
    Guid Id { get; set; }
}

public interface IRequestId<TRequest> : IRequestId
           where TRequest : IRequestId<TRequest>, new()
{
// Warning disabled as this format is required to parse parameter for Minimal API endpoint handler as part of AsParameters input
#pragma warning disable CA1000 // Do not declare static members on generic types

    static bool TryParse(string? input, out TRequest id)
    {
        if (Guid.TryParse(input, out Guid guid))
        {
            id = new()
            {
                Id = guid
            };
            return true;
        }

        id = default!;
        return false;
    }
#pragma warning restore CA1000 // Do not declare static members on generic type
}
