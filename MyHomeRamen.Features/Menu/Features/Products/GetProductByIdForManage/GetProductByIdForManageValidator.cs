using FluentValidation;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductByIdForManage;

public sealed class GetProductByIdForManageValidator : AbstractValidator<GetProductByIdForManageQuery>
{
    public GetProductByIdForManageValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID must not be empty.")
            .ChildRules(id =>
                id.RuleFor(id => id)
                    .MustAsync(async (id, ct) => await dbContext.Product.Exists(p => p.Id == (ProductId)id, ct))
                    .WithMessage("Product with the specified ID does not exist."));
    }
}
