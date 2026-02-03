namespace MyHomeRamen.Domain.Common.Product;

public static class ProductRules
{
    public static void CheckName(string name)
    {
        if (name.Length < ProductConstants.MinNameLength)
        {
            throw ProductErrors.NameTooShort();
        }

        if (name.Length > ProductConstants.MaxNameLength)
        {
            throw ProductErrors.NameTooLong();
        }
    }

    public static void CheckDescriptionLength(string description)
    {
        if (description.Length < ProductConstants.MinDescriptionLength)
        {
            throw ProductErrors.DescriptionTooShort();
        }

        if (description.Length > ProductConstants.MaxDescriptionLength)
        {
            throw ProductErrors.DescriptionTooLong();
        }
    }

    public static void CheckPriceRange(decimal price)
    {
        if (price < ProductConstants.MinPrice)
        {
            throw ProductErrors.PriceTooSmall();
        }

        if (price > ProductConstants.MaxPrice)
        {
            throw ProductErrors.PriceTooHigh();
        }
    }
}
