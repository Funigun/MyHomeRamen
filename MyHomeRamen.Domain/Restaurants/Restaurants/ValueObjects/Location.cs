namespace MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

public sealed class Location
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    private Location() { }

    public static Location Create(double latitude, double longitude)
    {
        Location location = new()
        {
            Latitude = latitude,
            Longitude = longitude
        };

        LocationValidator.Validate(location);
        return location;
    }
}
