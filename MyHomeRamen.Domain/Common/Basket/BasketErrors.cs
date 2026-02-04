namespace MyHomeRamen.Domain.Common.Basket;

public static class BasketErrors
{
    public static DomainException BasketUserRequired()
        => new("Basket must have a user assigned.");
}
