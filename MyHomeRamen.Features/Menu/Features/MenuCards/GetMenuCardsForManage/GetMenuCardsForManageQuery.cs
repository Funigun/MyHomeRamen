using FluentValidation;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;
using MyHomeRamen.Features.Menu.Features.MenuCards.Common;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardsForManage;

public sealed record GetMenuCardsForManageQuery(
    PageParameters PageParameters,
    GetMenuCardsForManageRequest Request)
    : IQuery<GetMenuCardsForManageResponse>;

public sealed record GetMenuCardsForManageQueryOptions(
    Guid RestaurantId,
    GetMenuCardsForManageRequest Request,
    PageParameters PageParameters)
    : PagedDbQueryOptions<MenuCard, MenuCardForManageDto>
    (
        new()
        {
            Filter = card =>
                card.RestaurantId == RestaurantId
                && (!Request.IsActive.HasValue || card.IsActive == Request.IsActive.Value)
                && (string.IsNullOrWhiteSpace(Request.Name)
                    || card.Name.ToLower().Contains(Request.Name.ToLower())),
            OrderBy = Request.SortBy?.ToUpperInvariant() switch
            {
                "NAME" => card => card.Name,
                "ISACTIVE" => card => card.IsActive,
                _ => card => card.SortOrder
            },
            OrderDirection = string.Equals(Request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc",
            PageNumber = PageParameters.PageNumber,
            PageSize = PageParameters.PageSize,
            Selector = card => new MenuCardForManageDto(
                card.Id.Value,
                card.RestaurantId,
                card.Name,
                card.IsActive,
                card.SortOrder)
        }
    );

public sealed class GetMenuCardsForManageAuthorizationPolicy(
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IAuthorizationPolicy<GetMenuCardsForManageQuery>
{
    public Task<bool> Authorize(
        GetMenuCardsForManageQuery request,
        CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanViewMenuCard));
}

public sealed class GetMenuCardsForManageValidator : AbstractValidator<GetMenuCardsForManageQuery>
{
    private static readonly string[] SortFields = ["Name", "IsActive", "SortOrder"];

    public GetMenuCardsForManageValidator()
    {
        RuleFor(query => query.PageParameters.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");
        RuleFor(query => query.PageParameters.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
        RuleFor(query => query.Request.Name)
            .MaximumLength(200)
            .WithMessage("Menu card name search must not exceed 200 characters.");
        RuleFor(query => query.Request.SortBy)
            .Must(sortBy => sortBy is null || SortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort field must be Name, IsActive, or SortOrder.");
        RuleFor(query => query.Request.SortOrder)
            .Must(sortOrder => sortOrder is null
                || sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                || sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Sort order must be asc or desc.");
    }
}

public sealed class GetMenuCardsForManageHandler(
    IMenuDbContext dbContext,
    ICompanyContext companyContext,
    ICurrentUser currentUser)
    : IRequestHandler<GetMenuCardsForManageQuery, GetMenuCardsForManageResponse>
{
    public async Task<GetMenuCardsForManageResponse> Handle(
        GetMenuCardsForManageQuery query,
        CancellationToken cancellationToken)
    {
        Guid restaurantId = companyContext.RestaurantId
            ?? throw new InvalidOperationException("A restaurant context is required for menu card management.");
        GetMenuCardsForManageQueryOptions options = new(
            restaurantId,
            query.Request,
            query.PageParameters);

        PagedResult<MenuCardForManageDto> result = await dbContext.MenuCard
            .Query()
            .ForManage(options, cancellationToken);

        MenuCardAllowedActions allowedActions = new(
            CanCreate: MenuAuthorization.CanManageRestaurant(
                currentUser,
                restaurantId,
                MenuPermissionConstants.CanManageMenuCards,
                MenuPermissionConstants.CanAddMenuCard),
            CanEdit: MenuAuthorization.CanManageRestaurant(
                currentUser,
                restaurantId,
                MenuPermissionConstants.CanManageMenuCards,
                MenuPermissionConstants.CanEditMenuCard),
            CanDelete: MenuAuthorization.CanManageRestaurant(
                currentUser,
                restaurantId,
                MenuPermissionConstants.CanManageMenuCards,
                MenuPermissionConstants.CanDeleteMenuCard));

        return new GetMenuCardsForManageResponse(
            query.PageParameters.PageNumber,
            query.PageParameters.PageSize,
            result.TotalItems,
            result.Items,
            allowedActions);
    }
}
