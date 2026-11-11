using FluentValidation;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;
using MyHomeRamen.Features.Menu.Features.Offers.Common;

namespace MyHomeRamen.Features.Menu.Features.Offers.GetRestaurantOffersForManage;

public sealed record GetRestaurantOffersForManageQuery(
    PageParameters PageParameters,
    GetRestaurantOffersForManageRequest Request)
    : IQuery<GetRestaurantOffersForManageResponse>;

public sealed record GetRestaurantOffersForManageQueryOptions(
    Guid RestaurantId,
    GetRestaurantOffersForManageRequest Request,
    PageParameters PageParameters,
    ProductId? ProductId,
    RestaurantProductOfferStatus? Status,
    bool CanEdit,
    bool CanRetire)
    : PagedDbQueryOptions<RestaurantProductOffer, RestaurantProductOfferForManageDto>
    (
        new()
        {
            Filter = offer =>
                offer.RestaurantId == RestaurantId
                && (!ProductId.HasValue || offer.ProductId == ProductId.Value)
                && (!Status.HasValue || offer.Status == Status.Value)
                && (string.IsNullOrWhiteSpace(Request.ProductName)
                    || offer.Product.Name.ToLower().Contains(Request.ProductName.ToLower())),
            OrderBy = Request.SortBy?.ToLowerInvariant() switch
            {
                "price" => offer => offer.Price,
                "status" => offer => offer.Status,
                _ => offer => offer.Product.Name
            },
            OrderDirection = string.Equals(Request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc",
            PageNumber = PageParameters.PageNumber,
            PageSize = PageParameters.PageSize,
            Selector = offer => new RestaurantProductOfferForManageDto(
                offer.Id.Value,
                offer.RestaurantId,
                offer.ProductId.Value,
                offer.Product.Name,
                offer.Price,
                offer.Status.ToString(),
                new OfferAllowedActions(
                    false,
                    CanEdit && offer.Status == RestaurantProductOfferStatus.Active,
                    CanRetire && offer.Status == RestaurantProductOfferStatus.Active))
        }
    );

public sealed class GetRestaurantOffersForManageAuthorizationPolicy(
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IAuthorizationPolicy<GetRestaurantOffersForManageQuery>
{
    public Task<bool> Authorize(
        GetRestaurantOffersForManageQuery request,
        CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageOffers,
            MenuPermissionConstants.CanViewOffer));
}

public sealed class GetRestaurantOffersForManageValidator
    : AbstractValidator<GetRestaurantOffersForManageQuery>
{
    private static readonly string[] SortFields = ["ProductName", "Price", "Status"];

    public GetRestaurantOffersForManageValidator()
    {
        RuleFor(query => query.PageParameters.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");
        RuleFor(query => query.PageParameters.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
        RuleFor(query => query.Request.ProductName)
            .MaximumLength(200)
            .WithMessage("Product search must not exceed 200 characters.");
        RuleFor(query => query.Request.ProductId)
            .Must(productId => !productId.HasValue || productId.Value != Guid.Empty)
            .WithMessage("Product ID must not be empty.");
        RuleFor(query => query.Request.Status)
            .Must(status => !status.HasValue
                || Enum.IsDefined(typeof(RestaurantProductOfferStatus), status.Value))
            .WithMessage("Offer status is invalid.");
        RuleFor(query => query.Request.SortBy)
            .Must(sortBy => sortBy is null || SortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort field must be ProductName, Price, or Status.");
        RuleFor(query => query.Request.SortOrder)
            .Must(sortOrder => sortOrder is null
                || sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                || sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Sort order must be asc or desc.");
    }
}

public sealed class GetRestaurantOffersForManageHandler(
    IMenuDbContext dbContext,
    ICompanyContext companyContext,
    ICurrentUser currentUser)
    : IRequestHandler<GetRestaurantOffersForManageQuery, GetRestaurantOffersForManageResponse>
{
    public async Task<GetRestaurantOffersForManageResponse> Handle(
        GetRestaurantOffersForManageQuery query,
        CancellationToken cancellationToken)
    {
        Guid restaurantId = companyContext.RestaurantId
            ?? throw new InvalidOperationException("A restaurant context is required for offer management.");
        bool canManageOffers = currentUser.HasPermission(
            "Menu",
            MenuPermissionConstants.CanManageOffers,
            restaurantId);
        bool canCreateOffer = canManageOffers && currentUser.HasPermission(
            "Menu",
            MenuPermissionConstants.CanAddOffer,
            restaurantId);
        bool canEditOffer = canManageOffers && currentUser.HasPermission(
            "Menu",
            MenuPermissionConstants.CanEditOffer,
            restaurantId);
        bool canRetireOffer = canManageOffers && currentUser.HasPermission(
            "Menu",
            MenuPermissionConstants.CanDeleteOffer,
            restaurantId);
        ProductId? productId = query.Request.ProductId is Guid requestedProductId
            ? new ProductId(requestedProductId)
            : null;
        GetRestaurantOffersForManageQueryOptions options = new(
            restaurantId,
            query.Request,
            query.PageParameters,
            productId,
            query.Request.Status,
            canEditOffer,
            canRetireOffer);

        PagedResult<RestaurantProductOfferForManageDto> result = await dbContext.RestaurantProductOffer
            .Query()
            .ForManage(options, cancellationToken);
        IReadOnlyList<EligibleProductForOfferDto> eligibleProducts = canCreateOffer
            ? await dbContext.Product.Query().EligibleForOffers(restaurantId, cancellationToken)
            : [];

        return new GetRestaurantOffersForManageResponse(
            query.PageParameters.PageNumber,
            query.PageParameters.PageSize,
            result.TotalItems,
            result.Items.ToArray(),
            eligibleProducts,
            new OfferAllowedActions(canCreateOffer, false, false));
    }
}
