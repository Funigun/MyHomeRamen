namespace MyHomeRamen.Domain.Common.Product;

public static class ProductConstants
{
    public const int MinNameLength = 5;

    public const int MaxNameLength = 100;

    public const int MinDescriptionLength = 15;

    public const int MaxDescriptionLength = 500;

    public const decimal MinPrice = 0.5m;

    public const decimal MaxPrice = 100.0m;

    public const int MaxImageUrlLength = 2048;
}
