namespace MyHomeRamen.Worker.DatabaseInitializer.Config;

internal sealed record DatabaseUserConfig
{
    public string Schema { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public string User { get; init; } = string.Empty;

    public string Password => $"{User}Password123";

    private DatabaseUserConfig(string schema, string role, string user)
    {
        Schema = schema;
        Role = role;
        User = user;
    }

    internal static DatabaseUserConfig CreateMenuAdmin()
    {
        return new
        (
            "menu",
            "MenuRole",
            "MenuAdmin"
        );
    }

    internal static DatabaseUserConfig CreateShoppingCartAdmin()
    {
        return new
        (
            "shoppingCart",
            "ShoppingCartRole",
            "ShoppingCartAdmin"
        );
    }

    internal static DatabaseUserConfig CreateOrderAdmin()
    {
        return new
        (
            "order",
            "OrderRole",
            "OrderAdmin"
        );
    }

    internal static DatabaseUserConfig CreateReservationAdmin()
    {
        return new
        (
            "reservation",
            "ReservationRole",
            "ReservationUser"
        );
    }

    internal static DatabaseUserConfig CreatePaymentAdmin()
    {
        return new
        (
            "payments",
            "PaymentRole",
            "PaymentUser"
        );
    }
}
