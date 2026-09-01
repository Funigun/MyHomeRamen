namespace MyHomeRamen.Domain.ShoppingCart.Baskets;

public enum BasketStatus
{
    None = 0,
    Active = 1,
    PendingOrder = 2,
    CheckedOut = 3,
    Abandoned = 4,
    Expired = 5
}
