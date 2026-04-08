using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage.Models;
using MyHomeRamen.Domain.Common.Product;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage.Policies;

public sealed class GetProductsForManageValidator : AbstractValidator<GetProductsForManageRequest>
{
    public GetProductsForManageValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(ProductConstants.MaxNameLength)
            .WithMessage($"Name must not exceed {ProductConstants.MaxNameLength} characters.");

        RuleFor(x => x.PriceFrom)
            .GreaterThanOrEqualTo(0)
            .WithMessage("PriceFrom must be a non-negative value.")
            .When(x => x.PriceFrom.HasValue);

        RuleFor(x => x.PriceTo)
            .GreaterThanOrEqualTo(0)
            .WithMessage("PriceTo must be a non-negative value.")
            .When(x => x.PriceTo.HasValue);

        RuleFor(x => x)
            .Must(x => !x.PriceFrom.HasValue || !x.PriceTo.HasValue || x.PriceFrom <= x.PriceTo)
            .WithMessage("PriceFrom must not exceed PriceTo.");
    }
}
