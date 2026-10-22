using FluentValidation;
using MyHomeRamen.Domain.Common.Restaurant;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.CreateInitialRestaurant;

public sealed record CreateInitialRestaurantRequest(
    string Name,
    string Street,
    string City,
    string ZipCode,
    double Latitude,
    double Longitude,
    string Phone,
    string Email,
    string AccountNumber,
    string BankName,
    string RoutingNumber);

public sealed record CreateInitialRestaurantResponse(Guid RestaurantId);

public sealed record CreateInitialRestaurantCommand(CreateInitialRestaurantRequest Request)
    : ICommand<CreateInitialRestaurantResponse>;

public sealed class CreateInitialRestaurantCommandValidator : AbstractValidator<CreateInitialRestaurantCommand>
{
    public CreateInitialRestaurantCommandValidator(
        IRestaurantDbContext restaurantDbContext,
        IIdentityService identityService)
    {
        RuleFor(command => command.Request.Name)
            .NotEmpty()
            .WithMessage("Restaurant name must not be empty.")
            .MaximumLength(200)
            .WithMessage("Restaurant name must not exceed 200 characters.")
            .MustAsync(async (name, cancellationToken) =>
                await restaurantDbContext.Restaurant.Query().IsNameUnique(name, cancellationToken))
            .WithMessage("Restaurant name must be unique.");

        RuleFor(command => command.Request.Street)
            .NotEmpty()
            .WithMessage("Street must not be empty.");

        RuleFor(command => command.Request.City)
            .NotEmpty()
            .WithMessage("City must not be empty.");

        RuleFor(command => command.Request.ZipCode)
            .NotEmpty()
            .WithMessage("ZIP code must not be empty.");

        RuleFor(command => command.Request.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(command => command.Request.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180.");

        RuleFor(command => command.Request.Phone)
            .NotEmpty()
            .WithMessage("Restaurant phone must not be empty.")
            .MaximumLength(RestaurantConstants.MaxPhoneLength)
            .WithMessage($"Restaurant phone must not exceed {RestaurantConstants.MaxPhoneLength} characters.");

        RuleFor(command => command.Request.Email)
            .NotEmpty()
            .WithMessage("Restaurant email must not be empty.")
            .MaximumLength(RestaurantConstants.MaxEmailLength)
            .WithMessage($"Restaurant email must not exceed {RestaurantConstants.MaxEmailLength} characters.");

        RuleFor(command => command.Request.AccountNumber)
            .NotEmpty()
            .WithMessage("Restaurant account number must not be empty.")
            .MaximumLength(RestaurantConstants.MaxAccountNumberLength)
            .WithMessage($"Restaurant account number must not exceed {RestaurantConstants.MaxAccountNumberLength} characters.");

        RuleFor(command => command.Request.BankName)
            .NotEmpty()
            .WithMessage("Restaurant bank name must not be empty.")
            .MaximumLength(RestaurantConstants.MaxBankNameLength)
            .WithMessage($"Restaurant bank name must not exceed {RestaurantConstants.MaxBankNameLength} characters.");

        RuleFor(command => command.Request.RoutingNumber)
            .NotEmpty()
            .WithMessage("Restaurant routing number must not be empty.")
            .MaximumLength(RestaurantConstants.MaxRoutingNumberLength)
            .WithMessage($"Restaurant routing number must not exceed {RestaurantConstants.MaxRoutingNumberLength} characters.");

        RuleFor(_ => _)
            .MustAsync(async (_, cancellationToken) =>
                await restaurantDbContext.Company.Exists(_ => true, cancellationToken))
            .WithMessage("A company must exist before its first restaurant can be created.");

        RuleFor(_ => _)
            .MustAsync(async (_, cancellationToken) =>
                await identityService.IsCompanyOwnerConfiguredAsync(cancellationToken))
            .WithMessage("A company owner must be registered before its first restaurant can be created.");

        RuleFor(_ => _)
            .MustAsync(async (_, cancellationToken) =>
                !await restaurantDbContext.Restaurant.Exists(_ => true, cancellationToken))
            .WithMessage("An initial restaurant already exists.");
    }
}

public sealed class CreateInitialRestaurantHandler(IRestaurantDbContext restaurantDbContext)
    : IRequestHandler<CreateInitialRestaurantCommand, CreateInitialRestaurantResponse>
{
    public async Task<CreateInitialRestaurantResponse> Handle(
        CreateInitialRestaurantCommand command,
        CancellationToken cancellationToken)
    {
        CreateInitialRestaurantRequest request = command.Request;
        Restaurant restaurant = Restaurant.Create(
            request.Name,
            Address.Create(request.Street, request.City, request.ZipCode, Location.Create(request.Latitude, request.Longitude)),
            true);

        restaurant.UpdateContactDetails(request.Phone, request.Email);
        restaurant.UpdateBankAccount(request.AccountNumber, request.BankName, request.RoutingNumber);

        Company company = await restaurantDbContext.Company.Load().Single(cancellationToken);
        company.AddRestaurant(restaurant);
        restaurantDbContext.Restaurant.Add(restaurant);
        await restaurantDbContext.SaveChangesAsync(cancellationToken);

        return new CreateInitialRestaurantResponse(restaurant.Id.Value);
    }
}
