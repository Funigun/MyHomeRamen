namespace MyHomeRamen.Domain.Venues.Restaurants.ValueObjects;

public sealed class ContactDetails
{
    public string Phone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    private ContactDetails() { }

    public static ContactDetails Create(string phone, string email)
    {
        return new ContactDetails
        {
            Phone = phone,
            Email = email
        };
    }
}
