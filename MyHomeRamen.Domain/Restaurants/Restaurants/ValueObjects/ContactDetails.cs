namespace MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

public sealed class ContactDetails
{
    public string Phone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    private ContactDetails() { }

    public static ContactDetails Create(string phone, string email)
    {
        ContactDetails contactDetails = new()
        {
            Phone = phone,
            Email = email
        };

        ContactDetailsValidator.Validate(contactDetails);
        return contactDetails;
    }
}
