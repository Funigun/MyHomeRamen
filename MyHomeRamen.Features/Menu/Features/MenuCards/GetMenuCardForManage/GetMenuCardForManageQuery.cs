using FluentValidation;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Domain.Menu.MenuSectionItems;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardForManage;

public sealed record GetMenuCardForManageQuery(Guid Id) : IQuery<GetMenuCardForManageResponse?>;

public sealed class GetMenuCardForManageAuthorizationPolicy(
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IAuthorizationPolicy<GetMenuCardForManageQuery>
{
    public Task<bool> Authorize(
        GetMenuCardForManageQuery request,
        CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanViewMenuCard));
}

public sealed class GetMenuCardForManageValidator : AbstractValidator<GetMenuCardForManageQuery>
{
    public GetMenuCardForManageValidator(IMenuDbContext dbContext, ICompanyContext companyContext)
    {
        RuleFor(query => query.Id)
            .NotEmpty()
            .WithMessage("Menu card ID must not be empty.")
            .MustAsync(async (id, cancellationToken) =>
            {
                Guid? restaurantId = companyContext.RestaurantId;
                return restaurantId.HasValue
                    && await dbContext.MenuCard.Exists(
                        card => card.Id == new MenuCardId(id) && card.RestaurantId == restaurantId.Value,
                        cancellationToken);
            })
            .WithMessage("Menu card must belong to the selected restaurant.");
    }
}

public sealed class GetMenuCardForManageHandler(
    IMenuDbContext dbContext,
    ICompanyContext companyContext)
    : IRequestHandler<GetMenuCardForManageQuery, GetMenuCardForManageResponse?>
{
    public async Task<GetMenuCardForManageResponse?> Handle(
        GetMenuCardForManageQuery query,
        CancellationToken cancellationToken)
    {
        Guid? restaurantId = companyContext.RestaurantId;
        if (!restaurantId.HasValue)
        {
            return null;
        }

        MenuCard? card = await dbContext.MenuCard.Query()
            .ByIdForManage(new MenuCardId(query.Id), restaurantId.Value, cancellationToken);
        if (card is null)
        {
            return null;
        }

        IEnumerable<MenuCardForManageSectionDto> sections = card.Sections
            .OrderBy(section => section.SortOrder)
            .Select(section => new MenuCardForManageSectionDto(
                section.Id.Value,
                section.Name,
                section.SortOrder,
                section.Items
                    .OrderBy(item => item.SortOrder)
                    .Select(MapOffer)));

        return new GetMenuCardForManageResponse(
            card.Id.Value,
            card.RestaurantId,
            card.Name,
            card.IsActive,
            sections);
    }

    private static MenuCardForManageOfferDto MapOffer(MenuSectionItem item)
    {
        RestaurantProductOffer offer = item.RestaurantProductOffer;
        Product product = offer.Product;
        IEnumerable<MenuCardForManageIngredientDto> ingredients = product.BaseIngredients
            .Select(ingredient => new MenuCardForManageIngredientDto(
                ingredient.Id.Value,
                ingredient.Name,
                false))
            .Concat(product.CustomIngredients.Select(
                ingredient => new MenuCardForManageIngredientDto(
                    ingredient.Id.Value,
                    ingredient.Name,
                    true)));

        return new MenuCardForManageOfferDto(
            offer.Id.Value,
            product.Id.Value,
            product.Name,
            product.Description,
            product.ImageUrl,
            offer.Price,
            offer.Status.ToString(),
            ingredients);
    }
}
