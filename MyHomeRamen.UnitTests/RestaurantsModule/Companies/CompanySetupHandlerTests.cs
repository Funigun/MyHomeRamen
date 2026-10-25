using System.Linq.Expressions;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Features.Companies.Common;
using MyHomeRamen.Features.Restaurants.Features.Companies.ConfigureCompany;
using MyHomeRamen.Features.Restaurants.Features.Companies.CreateInitialRestaurant;
using MyHomeRamen.Features.Restaurants.Features.Companies.RegisterOwner;
using MyHomeRamen.Features.Restaurants.Features.Companies.Setup;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;
using NSubstitute;

namespace MyHomeRamen.UnitTests.RestaurantsModule.Companies;

public sealed class CompanySetupHandlerTests
{
    [Theory]
    [InlineData(false, false, false, false)]
    [InlineData(true, false, false, false)]
    [InlineData(true, true, false, false)]
    [InlineData(true, true, true, true)]
    public async Task GetCompanySetupStatus_ShouldReportEachPersistedStep(
        bool ownerConfigured,
        bool companyExists,
        bool restaurantExists,
        bool isComplete)
    {
        IRestaurantDbContext restaurantDbContext = CreateDbContext(companyExists, restaurantExists);
        IIdentityService identityService = Substitute.For<IIdentityService>();
        identityService.IsCompanyOwnerConfiguredAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(ownerConfigured));
        GetCompanySetupStatusHandler handler = new(restaurantDbContext, identityService);

        CompanySetupStatusResponse response = await handler.Handle(
            new GetCompanySetupStatusQuery(),
            CancellationToken.None);

        Assert.Equal(ownerConfigured, response.IsOwnerConfigured);
        Assert.Equal(companyExists, response.CompanyExists);
        Assert.Equal(restaurantExists, response.RestaurantExists);
        Assert.Equal(isComplete, response.IsComplete);
    }

    [Fact]
    public async Task RegisterCompanyOwner_ShouldReturnLocalUserId_WhenIdentityRegistrationSucceeds()
    {
        Guid userId = Guid.CreateVersion7();
        IIdentityService identityService = Substitute.For<IIdentityService>();
        identityService.RegisterCompanyOwnerAsync(Arg.Any<CompanyOwnerDto>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new CompanyOwnerRegistrationResult(userId, null)));
        RegisterCompanyOwnerHandler handler = new(identityService);
        RegisterCompanyOwnerCommand command = new(new RegisterCompanyOwnerRequest(
            "owner",
            "Ramen",
            "Owner",
            "owner@example.com",
            "+48123456789",
            "Password1!",
            "Password1!"));

        RegisterCompanyOwnerResponse response = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(userId, response.UserId);
        await identityService.Received(1).RegisterCompanyOwnerAsync(
            Arg.Is<CompanyOwnerDto>(request =>
                request.UserName == command.Request.UserName
                && request.Email == command.Request.Email
                && request.Password == command.Request.Password),
            CancellationToken.None);
    }

    [Fact]
    public async Task ConfigureCompany_ShouldPersistCompany_WhenRequestIsValid()
    {
        IRestaurantDbContext restaurantDbContext = Substitute.For<IRestaurantDbContext>();
        ICompanyRepository companyRepository = Substitute.For<ICompanyRepository>();
        restaurantDbContext.Company.Returns(companyRepository);
        restaurantDbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));
        Company? savedCompany = null;
        companyRepository.Add(Arg.Do<Company>(company => savedCompany = company));
        ConfigureCompanyHandler handler = new(restaurantDbContext);
        ConfigureCompanyCommand command = new(
            new ConfigureCompanyRequest("My Home Ramen", "My Home Ramen Ltd", "PL123456789"));

        ConfigureCompanyResponse response = await handler.Handle(command, CancellationToken.None);

        Company persistedCompany = Assert.IsType<Company>(savedCompany);
        Assert.Equal(response.CompanyId, persistedCompany.Id.Value);
        Assert.Equal(command.Request.Name, persistedCompany.Name);
        await restaurantDbContext.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Theory]
    [InlineData(false, false, "A company owner must be registered before company configuration.")]
    [InlineData(true, true, "A company has already been configured for this deployment.")]
    public async Task ConfigureCompany_ShouldRejectRequest_WhenPrerequisiteIsNotMet(
        bool ownerConfigured,
        bool companyExists,
        string expectedError)
    {
        IRestaurantDbContext restaurantDbContext = Substitute.For<IRestaurantDbContext>();
        ICompanyRepository companyRepository = Substitute.For<ICompanyRepository>();
        ICompanyQuery companyQuery = Substitute.For<ICompanyQuery>();
        IIdentityService identityService = Substitute.For<IIdentityService>();
        restaurantDbContext.Company.Returns(companyRepository);
        companyRepository.Query().Returns(companyQuery);
        companyRepository.Exists(Arg.Any<Expression<Func<Company, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(companyExists));
        companyQuery.IsNameUnique(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        identityService.IsCompanyOwnerConfiguredAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(ownerConfigured));
        ConfigureCompanyCommandValidator validator = new(restaurantDbContext, identityService);
        ConfigureCompanyCommand command = new(
            new ConfigureCompanyRequest("My Home Ramen", "My Home Ramen Ltd", "PL123456789"));

        FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(command);

        Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
    }

    [Fact]
    public async Task CreateInitialRestaurant_ShouldRejectRequest_WhenOwnerIsNotConfigured()
    {
        IRestaurantDbContext restaurantDbContext = Substitute.For<IRestaurantDbContext>();
        ICompanyRepository companyRepository = Substitute.For<ICompanyRepository>();
        IRestaurantRepository restaurantRepository = Substitute.For<IRestaurantRepository>();
        IRestaurantQuery restaurantQuery = Substitute.For<IRestaurantQuery>();
        IIdentityService identityService = Substitute.For<IIdentityService>();
        restaurantDbContext.Company.Returns(companyRepository);
        restaurantDbContext.Restaurant.Returns(restaurantRepository);
        restaurantRepository.Query().Returns(restaurantQuery);
        companyRepository.Exists(Arg.Any<Expression<Func<Company, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        restaurantRepository.Exists(Arg.Any<Expression<Func<Restaurant, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        restaurantQuery.IsNameUnique(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        identityService.IsCompanyOwnerConfiguredAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        CreateInitialRestaurantCommandValidator validator = new(restaurantDbContext, identityService);
        CreateInitialRestaurantCommand command = new(new CreateInitialRestaurantRequest(
            "Downtown",
            "Main Street 1",
            "Warsaw",
            "00-001",
            52.2297,
            21.0122,
            "+48123456789",
            "restaurant@example.com",
            "123456789",
            "Example Bank",
            "routing-123"));

        FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(command);

        Assert.Contains(result.Errors, error =>
            error.ErrorMessage == "A company owner must be registered before its first restaurant can be created.");
    }

    [Theory]
    [InlineData("", "restaurant@example.com", "123456789", "Example Bank", "routing-123", "Restaurant phone must not be empty.")]
    [InlineData("+48123456789", "", "123456789", "Example Bank", "routing-123", "Restaurant email must not be empty.")]
    [InlineData("+48123456789", "restaurant@example.com", "", "Example Bank", "routing-123", "Restaurant account number must not be empty.")]
    [InlineData("+48123456789", "restaurant@example.com", "123456789", "", "routing-123", "Restaurant bank name must not be empty.")]
    [InlineData("+48123456789", "restaurant@example.com", "123456789", "Example Bank", "", "Restaurant routing number must not be empty.")]
    public async Task CreateInitialRestaurant_ShouldRejectRequest_WhenRequiredDetailsAreMissing(
        string phone,
        string email,
        string accountNumber,
        string bankName,
        string routingNumber,
        string expectedError)
    {
        IRestaurantDbContext restaurantDbContext = Substitute.For<IRestaurantDbContext>();
        ICompanyRepository companyRepository = Substitute.For<ICompanyRepository>();
        IRestaurantRepository restaurantRepository = Substitute.For<IRestaurantRepository>();
        IRestaurantQuery restaurantQuery = Substitute.For<IRestaurantQuery>();
        IIdentityService identityService = Substitute.For<IIdentityService>();
        restaurantDbContext.Company.Returns(companyRepository);
        restaurantDbContext.Restaurant.Returns(restaurantRepository);
        restaurantRepository.Query().Returns(restaurantQuery);
        companyRepository.Exists(Arg.Any<Expression<Func<Company, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        restaurantRepository.Exists(Arg.Any<Expression<Func<Restaurant, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        restaurantQuery.IsNameUnique(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        identityService.IsCompanyOwnerConfiguredAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        CreateInitialRestaurantCommandValidator validator = new(restaurantDbContext, identityService);
        CreateInitialRestaurantCommand command = new(new CreateInitialRestaurantRequest(
            "Downtown",
            "Main Street 1",
            "Warsaw",
            "00-001",
            52.2297,
            21.0122,
            phone,
            email,
            accountNumber,
            bankName,
            routingNumber));

        FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(command);

        Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
    }

    private static IRestaurantDbContext CreateDbContext(bool companyExists, bool restaurantExists)
    {
        IRestaurantDbContext restaurantDbContext = Substitute.For<IRestaurantDbContext>();
        ICompanyRepository companyRepository = Substitute.For<ICompanyRepository>();
        IRestaurantRepository restaurantRepository = Substitute.For<IRestaurantRepository>();

        restaurantDbContext.Company.Returns(companyRepository);
        restaurantDbContext.Restaurant.Returns(restaurantRepository);
        companyRepository.Exists(Arg.Any<Expression<Func<Company, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(companyExists));
        restaurantRepository.Exists(Arg.Any<Expression<Func<Restaurant, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(restaurantExists));

        return restaurantDbContext;
    }
}
