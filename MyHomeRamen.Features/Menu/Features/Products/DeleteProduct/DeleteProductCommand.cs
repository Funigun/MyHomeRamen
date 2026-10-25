using FluentValidation;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Products.DeleteProduct;

public sealed record DeleteProductCommand(ProductId Id) : ICommand;

public sealed class DeleteProductAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<DeleteProductCommand>
{
    public async Task<bool> Authorize(DeleteProductCommand request, CancellationToken cancellationToken)
        => await Task.FromResult(currentUser.CanManageProducts() && currentUser.CanDeleteProduct());
}

public sealed class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command.Id.Value)
            .MustBeValidProductId(dbContext);
    }
}

public sealed class DeleteProductHandler(IMenuDbContext dbContext) : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        Product product = await dbContext.Product.Load().ById(command.Id, cancellationToken);

        // TODO: Handle related deletion side effects in separate work.
        dbContext.Product.Delete(product);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
