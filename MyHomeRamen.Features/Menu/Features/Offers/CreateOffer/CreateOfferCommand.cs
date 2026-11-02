using FluentValidation;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.Offers.CreateOffer;

public sealed record CreateOfferCommand(Guid ProductId, decimal Price) : ICommand<CreateOfferResponse>;

public sealed class CreateOfferAuthorizationPolicy(ICurrentUser currentUser, ICompanyContext companyContext)
    : IAuthorizationPolicy<CreateOfferCommand>
{
    public Task<bool> Authorize(CreateOfferCommand request, CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageOffers,
            MenuPermissionConstants.CanAddOffer));
}

public sealed class CreateOfferValidator : AbstractValidator<CreateOfferCommand>
{
    public CreateOfferValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command.ProductId)
            .NotEmpty()
            .WithMessage("Product ID must not be empty.")
            .MustAsync(async (productId, cancellationToken) =>
                await dbContext.Product.Exists(
                    product => product.Id == new ProductId(productId) && product.Status == ProductStatus.Active,
                    cancellationToken))
            .WithMessage("An active product in this company must exist.");

        RuleFor(command => command.Price)
            .GreaterThan(0m)
            .WithMessage("Offer price must be greater than zero.");

        RuleFor(command => command.ProductId)
            .MustAsync(async (productId, cancellationToken) =>
                !await dbContext.RestaurantProductOffer.Exists(
                    offer => offer.RestaurantId == dbContext.RestaurantId && offer.ProductId == new ProductId(productId),
                    cancellationToken))
            .WithMessage("This product already has an offer at this restaurant.");
    }
}

public sealed class CreateOfferHandler(IMenuDbContext dbContext)
    : IRequestHandler<CreateOfferCommand, CreateOfferResponse>
{
    public async Task<CreateOfferResponse> Handle(CreateOfferCommand command, CancellationToken cancellationToken)
    {
        Product product = await dbContext.Product.Load().ById(new ProductId(command.ProductId), cancellationToken);
        RestaurantProductOffer offer = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            dbContext.RestaurantId,
            dbContext.CompanyId,
            product,
            command.Price);

        dbContext.RestaurantProductOffer.Add(offer);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateOfferResponse(offer.Id.Value);
    }
}
