using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.Identity.Api.Application;

public class RestaurantConfiguration
{
    public string ConnectionString { get; init; }

    public Guid RestaurantId { get; init; }

    private RestaurantConfiguration()
    {
    }

    public static RestaurantConfiguration Create(string connectionString, Guid id)
    {
        return new RestaurantConfiguration()
        {
            ConnectionString = connectionString,
            RestaurantId = id
        };
    }
}
