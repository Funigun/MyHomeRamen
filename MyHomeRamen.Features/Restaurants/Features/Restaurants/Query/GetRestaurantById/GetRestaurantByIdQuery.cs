using FluentValidation;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetRestaurantById;

public sealed record GetRestaurantByIdQuery(Guid RestaurantId) : IQuery<GetRestaurantByIdResponse>;
public sealed record GetRestaurantByIdResponse(RestaurantDetailsDto Restaurant);

public sealed class GetRestaurantByIdAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetRestaurantByIdQuery>
{
    public Task<bool> Authorize(GetRestaurantByIdQuery request, CancellationToken cancellationToken) => Task.FromResult(currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantManage));
}

public sealed class GetRestaurantByIdValidator : AbstractValidator<GetRestaurantByIdQuery>
{
    public GetRestaurantByIdValidator() => RuleFor(x => x.RestaurantId).NotEmpty().WithMessage("Restaurant identifier must not be empty.");
}

public sealed class GetRestaurantByIdHandler(IRestaurantDbContext dbContext) : IRequestHandler<GetRestaurantByIdQuery, GetRestaurantByIdResponse>
{
    public async Task<GetRestaurantByIdResponse> Handle(GetRestaurantByIdQuery query, CancellationToken cancellationToken)
    {
        RestaurantDetailsDto restaurant = await dbContext.Restaurant.Query().Details(query.RestaurantId, cancellationToken) ?? throw new KeyNotFoundException("Restaurant was not found.");
        return new GetRestaurantByIdResponse(restaurant);
    }
}
