using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Domain.ShoppingCart.Baskets;

public sealed class Basket : AuditableEntity, IEntity<BasketId>
{
    private readonly List<BasketItem> _items = [];

    public BasketId Id { get; private set; }

    public UserId UserId { get; private set; }

    public BasketStatus Status { get; private set; }

    public PaymentDetails.PaymentDetails? PaymentDetails { get; private set; } = default!;

    public ShippingDetails.ShippingDetails? ShippingDetails { get; private set; } = default!;

    public IReadOnlyList<BasketItem> Items => _items.ToList();

    private Basket() { }

    private Basket(BasketId id, UserId userId)
    {
        Id = id;
        UserId = userId;
        Status = BasketStatus.Active;
    }

    public static Basket Create(BasketId id, UserId userId)
    {
        Basket basket = new(id, userId);

        BasketValidator.Validate(basket);

        return basket;
    }

    public void AddItem(BasketItem item)
    {
        if (item is null)
        {
            throw BasketErrors.BasketItemProductRequired();
        }

        if (_items.Count >= BasketConstants.MaxProductsCount)
        {
            throw BasketErrors.BasketItemsLimitReached();
        }

        _items.Add(item);
    }

    public void RemoveItem(BasketItemId basketItemId)
    {
        BasketItem? item = _items.Find(i => i.Id == basketItemId);

        if (item is null)
        {
            throw BasketErrors.ItemNotFound();
        }

        _items.Remove(item);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public void UpdateShippingDetails(ShippingDetails.ShippingDetails details)
    {
        if (Status != BasketStatus.Active)
        {
            throw BasketErrors.BasketNotActive();
        }

        ShippingDetails = details;
    }

    public void UpdatePaymentDetails(PaymentDetails.PaymentDetails details)
    {
        if (Status != BasketStatus.Active)
        {
            throw BasketErrors.BasketNotActive();
        }

        PaymentDetails = details;
    }

    public void CheckOut() => Status = BasketStatus.CheckedOut;
    
}
