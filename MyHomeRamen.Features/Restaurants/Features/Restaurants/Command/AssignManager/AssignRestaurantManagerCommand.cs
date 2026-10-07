using FluentValidation;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Identity.Permissions;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Command.AssignManager;

public sealed record AssignRestaurantManagerRequest(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword,
    Guid CompanyId,
    string IdempotencyKey);
public sealed record AssignRestaurantManagerResponse(Guid MembershipId);
public sealed record AssignRestaurantManagerCommand(Guid RestaurantId, AssignRestaurantManagerRequest Request) : ICommand<AssignRestaurantManagerResponse>;

public sealed class AssignRestaurantManagerAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<AssignRestaurantManagerCommand>
{
    public async Task<bool> Authorize(AssignRestaurantManagerCommand request, CancellationToken cancellationToken) 
        => await Task.FromResult(currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantsManage));
}

public sealed class AssignRestaurantManagerValidator : AbstractValidator<AssignRestaurantManagerCommand>
{
    public AssignRestaurantManagerValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.Request.CompanyId).NotEmpty();
        RuleFor(x => x.Request.UserName).NotEmpty();
        RuleFor(x => x.Request.FirstName).NotEmpty();
        RuleFor(x => x.Request.LastName).NotEmpty();
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Request.PhoneNumber).NotEmpty();
        RuleFor(x => x.Request.Password).NotEmpty();
        RuleFor(x => x.Request.ConfirmPassword).Equal(x => x.Request.Password);
        RuleFor(x => x.Request.IdempotencyKey).NotEmpty().MaximumLength(200);
    }
}

public sealed class AssignRestaurantManagerHandler(IIdentityService identityService) : IRequestHandler<AssignRestaurantManagerCommand, AssignRestaurantManagerResponse>
{
    public async Task<AssignRestaurantManagerResponse> Handle(AssignRestaurantManagerCommand command, CancellationToken cancellationToken)
    {
        RestaurantAdminRegistrationResult result = await identityService.RegisterRestaurantAdminAsync(
            new RestaurantAdminRegistrationDto(
                command.Request.UserName,
                command.Request.FirstName,
                command.Request.LastName,
                command.Request.Email,
                command.Request.PhoneNumber,
                command.Request.Password,
                command.Request.ConfirmPassword,
                command.Request.CompanyId,
                command.RestaurantId,
                command.Request.IdempotencyKey),
            cancellationToken);

        if (result.Error is not null)
        {
            throw new ValidationException(result.Error);
        }

        if (result.MembershipId is not Guid membershipId)
        {
            throw new InvalidOperationException("Identity service returned an incomplete restaurant admin registration result.");
        }

        return new AssignRestaurantManagerResponse(membershipId);
    }
}
