using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Identity.Features.Companies.RegisterOwner;

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
        endpointBuilder.MapStandardPost<RegisterCompanyOwnerResponse>("api/identity/companies/register-owner", Handler)
                       .WithName("RegisterCompanyOwner")
                       .WithDescription("Registers company and assigns registering user as company owner.")
                       .WithTags("identity", "companies")
                       // ToDo: Authorization method to be detemined. E.g. personal link valid for 48h or until first login, etc., but how to generate links
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
        
        return TypedResults.Created($"/api/identity/companies/{response.CompanyId}", response);
    }
}
