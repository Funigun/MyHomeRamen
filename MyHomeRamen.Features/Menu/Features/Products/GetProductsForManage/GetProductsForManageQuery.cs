using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;

public sealed record GetProductsForManageQuery(PageParameters PageParameters, GetProductsForManageRequest Request) : IQuery<GetProductsForManageResponse>;

public sealed record GetProductsForManageQueryOptions(GetProductsForManageRequest Request, IEnumerable<CategoryId>? CategoryIds, IEnumerable<IngredientId>? IngredientIds, PageParameters PageParameters)
    : PagedDbQueryOptions<Product, ProductForManageDto>
    (
        new()
        {
            Filter = product => 
                (string.IsNullOrWhiteSpace(Request.Name) || product.Name.ToLower().Contains(Request.Name.ToLower())) &&
                (CategoryIds == null || product.Categories.Any(category => CategoryIds.Contains(category.Id))) &&
                (IngredientIds == null || product.BaseIngredients.Any(ingredient => IngredientIds.Contains(ingredient.Id)) || product.CustomIngredients.Any(ingredient => IngredientIds.Contains(ingredient.Id))) &&
                (Request.PriceFrom == null || product.Price >= Request.PriceFrom.Value) &&
                (Request.PriceTo == null || product.Price <= Request.PriceTo.Value),
            OrderBy = Request.OrderBy switch
            {
                "Price" => product => product.Price,
                _ => product => product.Name
            },
            OrderDirection = "asc",
            PageNumber = PageParameters.PageNumber,
            PageSize = PageParameters.PageSize,
            Selector = product => new ProductForManageDto(product.Id.Value, product.Name, product.Description, product.Price)
        }
    );

public sealed class GetProductsForManageAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetProductsForManageQuery>
{
    public async Task<bool> Authorize(GetProductsForManageQuery request, CancellationToken cancellationToken)
    {
        return currentUser.CanManageProducts();
    }
}

public sealed class GetProductsForManageValidator : AbstractValidator<GetProductsForManageQuery>
{
    public GetProductsForManageValidator()
    {
        When(x => !string.IsNullOrEmpty(x.Request.Name), () =>
        {
            RuleFor(x => x.Request.Name!)
                .MustNotExceedProductNameLength();
        });

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

public sealed class GetProductsForManageHandler(IMenuDbContext dbContext) : IRequestHandler<GetProductsForManageQuery, GetProductsForManageResponse>
{
    public async Task<GetProductsForManageResponse> Handle(
        GetProductsForManageQuery query,
        CancellationToken cancellationToken)
    {
        IEnumerable<CategoryId>? categoryIds = query.Request.CategoryIds?.Length > 0 ? query.Request.CategoryIds?.Select(c => new CategoryId(c)) : null;
        IEnumerable<IngredientId>? ingredientIds = query.Request.IngredientIds?.Length > 0 ? query.Request.IngredientIds?.Select(i => new IngredientId(i)) : null;

        GetProductsForManageQueryOptions options = new(query.Request, categoryIds, ingredientIds, query.PageParameters);

        PagedResult<ProductForManageDto> pagedResult = await dbContext.Product.Query().ForManage(options, cancellationToken);

        return new GetProductsForManageResponse(
            Page: query.PageParameters.PageNumber,
            PageSize: query.PageParameters.PageSize,
            TotalCount: pagedResult.TotalItems,
            Products: pagedResult.Items);
    }
}
