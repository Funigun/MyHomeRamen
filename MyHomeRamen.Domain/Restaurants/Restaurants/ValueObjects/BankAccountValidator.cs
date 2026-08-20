using MyHomeRamen.Domain.Common.Restaurant;

namespace MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

internal static class BankAccountValidator
{
    internal static void Validate(BankAccount bankAccount)
    {
        if (string.IsNullOrWhiteSpace(bankAccount.AccountNumber))
        {
            throw RestaurantErrors.AccountNumberRequired();
        }

        if (bankAccount.AccountNumber.Length > RestaurantConstants.MaxAccountNumberLength)
        {
            throw RestaurantErrors.AccountNumberTooLong();
        }

        if (string.IsNullOrWhiteSpace(bankAccount.BankName))
        {
            throw RestaurantErrors.BankNameRequired();
        }

        if (bankAccount.BankName.Length > RestaurantConstants.MaxBankNameLength)
        {
            throw RestaurantErrors.BankNameTooLong();
        }

        if (string.IsNullOrWhiteSpace(bankAccount.RoutingNumber))
        {
            throw RestaurantErrors.RoutingNumberRequired();
        }

        if (bankAccount.RoutingNumber.Length > RestaurantConstants.MaxRoutingNumberLength)
        {
            throw RestaurantErrors.RoutingNumberTooLong();
        }
    }
}
