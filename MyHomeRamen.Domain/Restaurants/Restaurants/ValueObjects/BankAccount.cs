namespace MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

public sealed class BankAccount
{
    public string AccountNumber { get; private set; } = string.Empty;
    public string BankName { get; private set; } = string.Empty;
    public string RoutingNumber { get; private set; } = string.Empty;

    private BankAccount() { }

    public static BankAccount Create(string accountNumber, string bankName, string routingNumber)
    {
        BankAccount bankAccount = new()
        {
            AccountNumber = accountNumber,
            BankName = bankName,
            RoutingNumber = routingNumber
        };

        BankAccountValidator.Validate(bankAccount);
        return bankAccount;
    }
}
