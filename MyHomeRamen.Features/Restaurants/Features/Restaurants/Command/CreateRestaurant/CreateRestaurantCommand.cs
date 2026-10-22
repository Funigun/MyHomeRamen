using FluentValidation;
using MyHomeRamen.Domain.Common.Restaurant;
using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Command.CreateRestaurant;

public sealed record CreateRestaurantRequest(string Name, bool IsActive, string Street, string City, string ZipCode, double Latitude, double Longitude, string Phone, string Email, string AccountNumber, string BankName, string RoutingNumber);
public sealed record CreateRestaurantResponse(Guid Id);
public sealed record CreateRestaurantCommand(CreateRestaurantRequest Request) : ICommand<CreateRestaurantResponse>;

public sealed class CreateRestaurantAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<CreateRestaurantCommand>
{
    public async Task<bool> Authorize(CreateRestaurantCommand request, CancellationToken cancellationToken) 
        => await Task.FromResult(currentUser.CanAddRestaurant());
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
        RuleFor(x => x.Request.Phone)
            .NotEmpty().WithMessage("Restaurant phone must not be empty.")
            .MaximumLength(RestaurantConstants.MaxPhoneLength)
            .WithMessage($"Restaurant phone must not exceed {RestaurantConstants.MaxPhoneLength} characters.");
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Restaurant email must not be empty.")
            .MaximumLength(RestaurantConstants.MaxEmailLength)
            .WithMessage($"Restaurant email must not exceed {RestaurantConstants.MaxEmailLength} characters.");
        RuleFor(x => x.Request.AccountNumber)
            .NotEmpty().WithMessage("Restaurant account number must not be empty.")
            .MaximumLength(RestaurantConstants.MaxAccountNumberLength)
            .WithMessage($"Restaurant account number must not exceed {RestaurantConstants.MaxAccountNumberLength} characters.");
        RuleFor(x => x.Request.BankName)
            .NotEmpty().WithMessage("Restaurant bank name must not be empty.")
            .MaximumLength(RestaurantConstants.MaxBankNameLength)
            .WithMessage($"Restaurant bank name must not exceed {RestaurantConstants.MaxBankNameLength} characters.");
        RuleFor(x => x.Request.RoutingNumber)
            .NotEmpty().WithMessage("Restaurant routing number must not be empty.")
            .MaximumLength(RestaurantConstants.MaxRoutingNumberLength)
            .WithMessage($"Restaurant routing number must not exceed {RestaurantConstants.MaxRoutingNumberLength} characters.");
    }
}

public sealed class CreateRestaurantHandler(IRestaurantDbContext dbContext) : IRequestHandler<CreateRestaurantCommand, CreateRestaurantResponse>
{
    public async Task<CreateRestaurantResponse> Handle(CreateRestaurantCommand command, CancellationToken cancellationToken)
    {
        CreateRestaurantRequest request = command.Request;
        Restaurant restaurant = Restaurant.Create(request.Name, Address.Create(request.Street, request.City, request.ZipCode, Location.Create(request.Latitude, request.Longitude)), request.IsActive);

        restaurant.UpdateContactDetails(request.Phone, request.Email);
        restaurant.UpdateBankAccount(request.AccountNumber, request.BankName, request.RoutingNumber);

        dbContext.Restaurant.Add(restaurant);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(restaurant.Id.Value);
    }
}
