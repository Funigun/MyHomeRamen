using FluentValidation;
using MyHomeRamen.Domain.Common.Product;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage;

public sealed class GetProductsForManageValidator : AbstractValidator<GetProductsForManageQuery>
{
    public GetProductsForManageValidator()
    {
        RuleFor(x => x.Request.Name)
            .MaximumLength(ProductConstants.MaxNameLength)
            .WithMessage($"Name must not exceed {ProductConstants.MaxNameLength} characters.");

        RuleFor(x => x.Request.PriceFrom)
            .GreaterThanOrEqualTo(0)
            .WithMessage("PriceFrom must be a non-negative value.")
            .When(x => x.Request.PriceFrom.HasValue);

        RuleFor(x => x.Request.PriceTo)
            .GreaterThanOrEqualTo(0)
            .WithMessage("PriceTo must be a non-negative value.")
            .When(x => x.Request.PriceTo.HasValue);

        RuleFor(x => x)
            .Must(x => !x.Request.PriceFrom.HasValue || !x.Request.PriceTo.HasValue || x.Request.PriceFrom <= x.Request.PriceTo)
            .WithMessage("PriceFrom must not exceed PriceTo.");
    }
}
