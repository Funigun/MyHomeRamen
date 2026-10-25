using FluentValidation;
using Microsoft.Extensions.Localization;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Menu.Products;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Models;

public sealed class ProductFormModelValidator : BaseValidator<ProductFormModel>
{
    public ProductFormModelValidator(IStringLocalizer<ProductsResources> text)
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage(text["NameRequired"])
            .MinimumLength(5).WithMessage(text["NameMinLength"])
            .MaximumLength(100).WithMessage(text["NameMaxLength"]);

        RuleFor(model => model.Description)
            .NotEmpty().WithMessage(text["DescriptionRequired"])
            .MinimumLength(15).WithMessage(text["DescriptionMinLength"])
            .MaximumLength(500).WithMessage(text["DescriptionMaxLength"]);

        RuleFor(model => model.Price)
            .InclusiveBetween(0.5m, 100.0m).WithMessage(text["PriceRange"]);

        RuleFor(model => model.CategoryId)
            .NotEmpty().WithMessage(text["CategoryRequired"]);

        RuleFor(model => model.IngredientIds)
            .NotEmpty().WithMessage(text["IngredientRequired"]);
    }
}
