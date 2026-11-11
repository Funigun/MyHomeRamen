using FluentValidation;
using MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;
using MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.Common;

namespace MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.GetRestaurantIngredientPricesForManage;

public sealed record GetRestaurantIngredientPricesForManageQuery(
    PageParameters PageParameters,
    GetRestaurantIngredientPricesForManageRequest Request)
    : IQuery<GetRestaurantIngredientPricesForManageResponse>;

public sealed record GetRestaurantIngredientPricesForManageQueryOptions(
    Guid RestaurantId,
    GetRestaurantIngredientPricesForManageRequest Request,
    PageParameters PageParameters)
    : PagedDbQueryOptions<RestaurantIngredientPrice, RestaurantIngredientPriceForManageDto>
    (
        new()
        {
            Filter = price =>
                price.RestaurantId == RestaurantId
                && (string.IsNullOrWhiteSpace(Request.IngredientName)
                    || price.Ingredient.Name.ToLower().Contains(Request.IngredientName.ToLower())),
            OrderBy = string.Equals(Request.SortBy, "Price", StringComparison.OrdinalIgnoreCase)
                ? price => price.Price
                : price => price.Ingredient.Name,
            OrderDirection = string.Equals(Request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc",
            PageNumber = PageParameters.PageNumber,
            PageSize = PageParameters.PageSize,
            Selector = price => new RestaurantIngredientPriceForManageDto(
                price.Id.Value,
                price.RestaurantId,
                price.IngredientId.Value,
                price.Ingredient.Name,
                price.Price)
        }
    );

public sealed class GetRestaurantIngredientPricesForManageAuthorizationPolicy(
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IAuthorizationPolicy<GetRestaurantIngredientPricesForManageQuery>
{
    public Task<bool> Authorize(
        GetRestaurantIngredientPricesForManageQuery request,
        CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageIngredientPrices,
            MenuPermissionConstants.CanViewIngredientPrice));
}

public sealed class GetRestaurantIngredientPricesForManageValidator
    : AbstractValidator<GetRestaurantIngredientPricesForManageQuery>
{
    private static readonly string[] SortFields = ["IngredientName", "Price"];

    public GetRestaurantIngredientPricesForManageValidator()
    {
        RuleFor(query => query.PageParameters.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");
        RuleFor(query => query.PageParameters.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
        RuleFor(query => query.Request.IngredientName)
            .MaximumLength(200)
            .WithMessage("Ingredient search must not exceed 200 characters.");
        RuleFor(query => query.Request.SortBy)
            .Must(sortBy => sortBy is null || SortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort field must be IngredientName or Price.");
        RuleFor(query => query.Request.SortOrder)
            .Must(sortOrder => sortOrder is null
                || sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                || sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Sort order must be asc or desc.");
    }
}

public sealed class GetRestaurantIngredientPricesForManageHandler(
    IMenuDbContext dbContext,
    ICompanyContext companyContext,
    ICurrentUser currentUser)
    : IRequestHandler<GetRestaurantIngredientPricesForManageQuery, GetRestaurantIngredientPricesForManageResponse>
{
    public async Task<GetRestaurantIngredientPricesForManageResponse> Handle(
        GetRestaurantIngredientPricesForManageQuery query,
        CancellationToken cancellationToken)
    {
        Guid restaurantId = companyContext.RestaurantId
            ?? throw new InvalidOperationException("A restaurant context is required for ingredient price management.");
        GetRestaurantIngredientPricesForManageQueryOptions options = new(
            restaurantId,
            query.Request,
            query.PageParameters);

        PagedResult<RestaurantIngredientPriceForManageDto> result = await dbContext.RestaurantIngredientPrice
            .Query()
            .ForManage(options, cancellationToken);

        IngredientPriceAllowedActions allowedActions = new(
            CanCreate: MenuAuthorization.CanManageRestaurant(
                currentUser,
                restaurantId,
                MenuPermissionConstants.CanManageIngredientPrices,
                MenuPermissionConstants.CanAddIngredientPrice),
            CanEdit: MenuAuthorization.CanManageRestaurant(
                currentUser,
                restaurantId,
                MenuPermissionConstants.CanManageIngredientPrices,
                MenuPermissionConstants.CanEditIngredientPrice),
            CanRemove: MenuAuthorization.CanManageRestaurant(
                currentUser,
                restaurantId,
                MenuPermissionConstants.CanManageIngredientPrices,
                MenuPermissionConstants.CanDeleteIngredientPrice));

        return new GetRestaurantIngredientPricesForManageResponse(
            query.PageParameters.PageNumber,
            query.PageParameters.PageSize,
            result.TotalItems,
            result.Items,
            allowedActions);
    }
}
