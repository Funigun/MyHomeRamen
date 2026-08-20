using MyHomeRamen.Domain.Common.Restaurant;

namespace MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

internal static class ContactDetailsValidator
{
    internal static void Validate(ContactDetails contactDetails)
    {
        if (string.IsNullOrWhiteSpace(contactDetails.Phone))
        {
            throw RestaurantErrors.PhoneRequired();
        }

        if (contactDetails.Phone.Length > RestaurantConstants.MaxPhoneLength)
        {
            throw RestaurantErrors.PhoneTooLong();
        }

        if (string.IsNullOrWhiteSpace(contactDetails.Email))
        {
            throw RestaurantErrors.EmailRequired();
        }

        if (contactDetails.Email.Length > RestaurantConstants.MaxEmailLength)
        {
            throw RestaurantErrors.EmailTooLong();
        }
    }
}
