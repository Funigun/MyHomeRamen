using System.Linq.Expressions;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Exceptions;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Features.Companies.Common;
using MyHomeRamen.Features.Restaurants.Features.Companies.RegisterOwner;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

namespace MyHomeRamen.UnitTests.RestaurantsModule.Companies.RegisterOwner;

public sealed class RegisterCompanyOwnerHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateCompany_WhenIdentityRegistrationSucceeds()
    {
        Guid companyId = Guid.CreateVersion7();
        IdentityServiceStub identityService = new(new CompanyOwnerRegistrationResult(Guid.CreateVersion7(), companyId, null));
        RestaurantDbContextStub restaurantDbContext = new();
        RegisterCompanyOwnerHandler handler = new(identityService, restaurantDbContext);
        RegisterCompanyOwnerCommand command = new(
            new RegisterCompanyOwnerRequest("owner", "First", "Last", "owner@example.com", "123456789", "password", "password", "Ramen House"),
            "idempotency-key");

        RegisterCompanyOwnerResponse response = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(companyId, response.CompanyId);
        Assert.NotNull(identityService.RequestedDto);
        Assert.Equal("idempotency-key", identityService.RequestedDto.IdempotencyKey);
        Assert.Equal(companyId, identityService.RequestedDto.CompanyId);
        Assert.Equal(1, restaurantDbContext.CompanyRepository.AddedCompanies.Count);
        Assert.Equal(companyId, restaurantDbContext.CompanyRepository.AddedCompanies.Single().Id.Value);
        Assert.Equal(1, restaurantDbContext.SaveChangesCount);
    }

    [Fact]
    public async Task Handle_ShouldNotCreateCompany_WhenIdentityRegistrationFails()
    {
        IdentityServiceStub identityService = new(new CompanyOwnerRegistrationResult(null, null, "Registration failed."));
        RestaurantDbContextStub restaurantDbContext = new();
        RegisterCompanyOwnerHandler handler = new(identityService, restaurantDbContext);
        RegisterCompanyOwnerCommand command = new(
            new RegisterCompanyOwnerRequest("owner", "First", "Last", "owner@example.com", "123456789", "password", "password", "Ramen House"),
            "idempotency-key");

        FluentValidation.ValidationException exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("Registration failed.", exception.Message);
        Assert.Empty(restaurantDbContext.CompanyRepository.AddedCompanies);
        Assert.Equal(0, restaurantDbContext.SaveChangesCount);
    }

    [Fact]
    public async Task Handle_ShouldRejectDuplicateCompanyName_WhenIdentityRegistrationIsNew()
    {
        Guid conflictingCompanyId = Guid.CreateVersion7();
        RestaurantDbContextStub restaurantDbContext = new();
        restaurantDbContext.CompanyRepository.Seed(Company.Create(conflictingCompanyId, "Ramen House", null, null));
        IdentityServiceStub identityService = new(new CompanyOwnerRegistrationResult(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            null));
        RegisterCompanyOwnerHandler handler = new(identityService, restaurantDbContext);
        RegisterCompanyOwnerCommand command = new(
            new RegisterCompanyOwnerRequest("owner", "First", "Last", "owner@example.com", "123456789", "password", "password", "Ramen House"),
            "idempotency-key");

        ConflictException exception = await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("Company name must be unique.", exception.Message);
        Assert.Empty(restaurantDbContext.CompanyRepository.AddedCompanies);
        Assert.Equal(0, restaurantDbContext.SaveChangesCount);
    }

    [Fact]
    public async Task Handle_ShouldNotCreateDuplicateCompany_WhenIdentityRegistrationIsRetried()
    {
        Guid companyId = Guid.CreateVersion7();
        IdentityServiceStub identityService = new(new CompanyOwnerRegistrationResult(Guid.CreateVersion7(), companyId, null));
        RestaurantDbContextStub restaurantDbContext = new();
        restaurantDbContext.CompanyRepository.Seed(Company.Create(companyId, "Ramen House", null, null));
        RegisterCompanyOwnerHandler handler = new(identityService, restaurantDbContext);
        RegisterCompanyOwnerCommand command = new(
            new RegisterCompanyOwnerRequest("owner", "First", "Last", "owner@example.com", "123456789", "password", "password", "Ramen House"),
            "idempotency-key");

        RegisterCompanyOwnerResponse response = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(companyId, response.CompanyId);
        Assert.Empty(restaurantDbContext.CompanyRepository.AddedCompanies);
        Assert.Equal(0, restaurantDbContext.SaveChangesCount);
    }

    private sealed class IdentityServiceStub(CompanyOwnerRegistrationResult result) : IIdentityService
    {
        public CompanyOwnerDto? RequestedDto { get; private set; }

        public Task<CompanyOwnerRegistrationResult> RegisterCompanyOwnerAsync(CompanyOwnerDto companyOwner, CancellationToken cancellationToken)
        {
            RequestedDto = companyOwner;
            return Task.FromResult(result);
        }

        public Task<RestaurantAdminRegistrationResult> RegisterRestaurantAdminAsync(RestaurantAdminRegistrationDto restaurantAdmin, CancellationToken cancellationToken)
            => throw new NotSupportedException();
    }

    private sealed class RestaurantDbContextStub : IRestaurantDbContext
    {
        public CompanyRepositoryStub CompanyRepository { get; } = new();

        public int SaveChangesCount { get; private set; }

        public ICompanyRepository Company => CompanyRepository;

        public IRestaurantRepository Restaurant => throw new NotSupportedException();

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCount++;
            return Task.FromResult(1);
        }

        public Task<bool> EnsureCreated(CancellationToken cancellationToken) => Task.FromResult(false);

        public Task Migrate(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken) => Task.FromResult(0);
    }

    private sealed class CompanyRepositoryStub : ICompanyRepository, ICompanyQuery
    {
        private readonly List<Company> _companies = [];

        public List<Company> AddedCompanies { get; } = [];

        public ICompanyQuery Query() => this;

        public ICompanyLoader Load() => throw new NotSupportedException();

        public void Add(Company entity)
        {
            AddedCompanies.Add(entity);
            _companies.Add(entity);
        }

        public void AddRange(IEnumerable<Company> entities) => throw new NotSupportedException();

        public void Update(Company entity) => throw new NotSupportedException();

        public void UpdateRange(IEnumerable<Company> entities) => throw new NotSupportedException();

        public Task<bool> Exists(Expression<Func<Company, bool>> predicate, CancellationToken cancellationToken)
            => Task.FromResult(_companies.AsQueryable().Any(predicate));

        public void Delete(Company entity) => throw new NotSupportedException();

        public Task<int> ExecuteDelete(Expression<Func<Company, bool>> predicate, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<int> ExecuteUpdate(
            Expression<Func<Company, object>> filterPredicate,
            Dictionary<Expression<Func<Company, object>>, Expression> valuesToUpdate,
            CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<int> Count(CancellationToken cancellationToken) => Task.FromResult(_companies.Count);

        public Task<bool> IsNameUnique(string normalizedName, CancellationToken cancellationToken)
            => Task.FromResult(_companies.All(company => company.NormalizedName != normalizedName));

        public void Seed(Company company) => _companies.Add(company);
    }
}
