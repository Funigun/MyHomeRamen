using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.RegisterOwner;

public sealed record RegisterCompanyOwnerRequest(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword,
    string CompanyName);

public sealed record RegisterCompanyOwnerResponse(Guid CompanyId);

public sealed class RegisterCompanyOwnerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<RegisterCompanyOwnerResponse>("api/restaurants/company/register-owner", Handler)
                       .ProducesProblem(StatusCodes.Status409Conflict)
                       .WithName("RegisterCompanyOwner")
                       .WithDescription("Registers company and assigns registering user as company owner.")
                       .WithTags("restaurants", "companies")
                       .AllowAnonymous();
    }

    private static async Task<IResult> Handler(
        [FromBody] RegisterCompanyOwnerRequest request,
        [FromHeader(Name = HeaderConstants.IdempotencyKey)] string? idempotencyKey,
        [FromServices] IRequestHandler<RegisterCompanyOwnerCommand, RegisterCompanyOwnerResponse> handler,
        CancellationToken cancellationToken)
    {
        RegisterCompanyOwnerCommand command = new(request, idempotencyKey ?? string.Empty);
        RegisterCompanyOwnerResponse response = await handler.Handle(command, cancellationToken);
        
        return TypedResults.Created($"/api/restaurants/companies/{response.CompanyId}", response);
    }
}
