using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Contracts;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;

public sealed record GetCategoriesByTypeQuery(GetCategoriesByTypeRequest Request) : IQuery<GetCategoriesByTypeResponse>;

public record GetCategoryByTypeQueryOptions(CategoryType CategoryType)
            : DbQueryOptions<Category, CategoryByTypeDto>
              (
                    new()
                    {
                        OrderBy = c => c.SortOrder,
                        OrderDirection = "asc",
                        Filter = c => c.CategoryType == CategoryType,
                        Selector = c => new CategoryByTypeDto(c.Id, c.Name, c.SortOrder)
                    }
              );

public sealed class GetCategoriesByTypeAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetCategoriesByTypeQuery>
{
    public Task<bool> Authorize(GetCategoriesByTypeQuery request, CancellationToken cancellationToken)
    {
        bool isAuthorized = request.Request.CategoryType switch
        {
            (int)CategoryType.Product => currentUser.CanManageCategories() || currentUser.CanManageProducts(),
            (int)CategoryType.Ingredient => currentUser.CanManageCategories() || currentUser.CanManageIngredients(),
            _ => currentUser.CanManageCategories()
        };

        return Task.FromResult(isAuthorized);
    }
}

public sealed class GetCategoriesByTypeValidator : AbstractValidator<GetCategoriesByTypeQuery>
{
    public GetCategoriesByTypeValidator()
    {
        RuleFor(x => x.Request.CategoryType)
            .MustBeValidCategoryType();
    }
}

public sealed class GetCategoriesByTypeHandler(IMenuDbContext dbContext, ICurrentUser currentUser)
                  : IRequestHandler<GetCategoriesByTypeQuery, GetCategoriesByTypeResponse>
{
    public async Task<GetCategoriesByTypeResponse> Handle(GetCategoriesByTypeQuery query, CancellationToken cancellationToken)
    {
        GetCategoryByTypeQueryOptions options = new((CategoryType)query.Request.CategoryType);

        IEnumerable<CategoryByTypeDto> categories = await dbContext.Category.Query().GetByTypeDto(options, cancellationToken);

        return new GetCategoriesByTypeResponse(
            categories,
            new AllowedActionsDto(
                CanCreate: currentUser.CanAddCategory(),
                CanEdit: currentUser.CanEditCategory(),
                CanDelete: currentUser.CanDeleteCategory()));
    }
}
