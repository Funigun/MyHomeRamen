using FluentValidation;
using MyHomeRamen.Blazor.Common.Models;
using MyHomeRamen.Common.Contracts.Menu.Products.Validators;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Components;

public sealed class ProductValidator : BaseValidator<ProductModel>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .SetValidator(new ProductNameValidator());

        RuleFor(x => x.Description)
            .SetValidator(new ProductDescriptionValidator()!);

        RuleFor(x => x.Price)
            .SetValidator(new ProductPriceValidator());

        RuleFor(x => x.CategoryId)
            .NotNull()
            .WithMessage("Please select a category.");

        RuleFor(x => x.IngredientIds)
            .NotEmpty()
            .WithMessage("Please select at least one ingredient.");
    }
}
