using FluentValidation;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductById;

public sealed class GetProductByIdValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdValidator(IMenuDbContext menuDbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID must not be empty.")
            .ChildRules(id =>
                id.RuleFor(id => id)
                    .MustAsync(async (id, ct) => await menuDbContext.Product.Exists(p => p.Id == (ProductId)id, ct))
                    .WithMessage("Product with the specified ID does not exist."));
    }
}
