using FluentValidation;
using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Identity.Permissions;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Command.CreateRestaurant;

public sealed record CreateRestaurantRequest(string Name, bool IsActive, string Street, string City, string ZipCode, double Latitude, double Longitude, string? Phone, string? Email, string? AccountNumber, string? BankName, string? RoutingNumber);
public sealed record CreateRestaurantResponse(Guid Id);
public sealed record CreateRestaurantCommand(CreateRestaurantRequest Request) : ICommand<CreateRestaurantResponse>;

public sealed class CreateRestaurantAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<CreateRestaurantCommand>
{
    public async Task<bool> Authorize(CreateRestaurantCommand request, CancellationToken cancellationToken) 
        => await Task.FromResult(currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantsCreate));
}

public sealed class CreateRestaurantValidator : AbstractValidator<CreateRestaurantCommand>
{
    public CreateRestaurantValidator(IRestaurantDbContext dbContext)
    {
        RuleFor(x => x.Request.Name).NotEmpty()
            .WithMessage("Restaurant name must not be empty.")
            .MaximumLength(200).WithMessage("Restaurant name must not exceed 200 characters.")
            .MustAsync(async (name, cancellationToken) => await dbContext.Restaurant.Query().IsNameUnique(name, cancellationToken)).WithMessage("Restaurant name must be unique.");
        RuleFor(x => x.Request.Street).NotEmpty();
        RuleFor(x => x.Request.City).NotEmpty();
        RuleFor(x => x.Request.ZipCode).NotEmpty();
    }
}

public sealed class CreateRestaurantHandler(IRestaurantDbContext dbContext) : IRequestHandler<CreateRestaurantCommand, CreateRestaurantResponse>
{
    public async Task<CreateRestaurantResponse> Handle(CreateRestaurantCommand command, CancellationToken cancellationToken)
    {
        CreateRestaurantRequest request = command.Request;
        Restaurant restaurant = Restaurant.Create(request.Name, Address.Create(request.Street, request.City, request.ZipCode, Location.Create(request.Latitude, request.Longitude)), request.IsActive);

        if (!string.IsNullOrWhiteSpace(request.Phone) && !string.IsNullOrWhiteSpace(request.Email))
        {
            restaurant.UpdateContactDetails(request.Phone, request.Email);
        }

        if (!string.IsNullOrWhiteSpace(request.AccountNumber) && !string.IsNullOrWhiteSpace(request.BankName) && !string.IsNullOrWhiteSpace(request.RoutingNumber))
        {
            restaurant.UpdateBankAccount(request.AccountNumber, request.BankName, request.RoutingNumber);
        }

        dbContext.Restaurant.Add(restaurant);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(restaurant.Id.Value);
    }
}
