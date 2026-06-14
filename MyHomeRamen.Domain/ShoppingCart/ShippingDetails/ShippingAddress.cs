namespace MyHomeRamen.Domain.ShoppingCart.ShippingDetails;

public sealed record ShippingAddress(
    string Street,
    string Building,
    string Apartment,
    string City,
    string ZipCode
);
