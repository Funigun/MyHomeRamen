using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed record GetIngredientsForManageQuery(GetIngredientsForManageRequest Request, PageParameters PageParameters) : IQuery<GetIngredientsForManageResponse>;

public sealed record GetIngredientsForManageQueryOptions(string? Name, IEnumerable<CategoryId>? CategoryIds, PageParameters PageParameters) 
                    : PagedDbQueryOptions<Ingredient, IngredientForManageDto>
                    (
                        new()
                        {
                            Filter = ingredient =>
                                (string.IsNullOrWhiteSpace(Name) || ingredient.Name.ToLower().Contains(Name.ToLower())) &&
                                (CategoryIds == null || ingredient.Categories.Any(category => CategoryIds.Contains(category.Id))),
                            OrderBy = ingredient => ingredient.Name,
                            OrderDirection = "asc",
                            PageNumber = PageParameters.PageNumber,
                            PageSize = PageParameters.PageSize,
                            Selector = ingredient => new IngredientForManageDto(ingredient.Id.Value, ingredient.Name, ingredient.Description)
                        }
                    );

public sealed class GetIngredientsForManageAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetIngredientsForManageQuery>
{
    public async Task<bool> Authorize(GetIngredientsForManageQuery request, CancellationToken cancellationToken)
    {
        return currentUser.CanManageIngredients();
    }
}

public sealed class GetIngredientsForManageValidator : AbstractValidator<GetIngredientsForManageQuery>
{
    public GetIngredientsForManageValidator()
    {
        When(x => x.Request.Name is not null, () =>
        {
            RuleFor(x => x.Request.Name!)
                .MustNotExceedIngredientNameLength();
        });
    }
}

public sealed class GetIngredientsForManageHandler(IMenuDbContext dbContext) : IRequestHandler<GetIngredientsForManageQuery, GetIngredientsForManageResponse>
{
    public async Task<GetIngredientsForManageResponse> Handle(GetIngredientsForManageQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<CategoryId>? categoryIds = query.Request.CategoryIds?.Length > 0 ? query.Request.CategoryIds.Select(c => new CategoryId(c)) : null;

        GetIngredientsForManageQueryOptions options = new(query.Request.Name, categoryIds, query.PageParameters);

        PagedResult<IngredientForManageDto> result = await dbContext.Ingredient.Query().ForManage(options, cancellationToken);

        return new GetIngredientsForManageResponse
        (
            Page: query.PageParameters.PageNumber,
            PageSize: query.PageParameters.PageSize,
            TotalCount: result.TotalItems,
            Ingredients: result.Items
        );
    }
}
