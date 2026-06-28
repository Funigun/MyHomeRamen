using FluentValidation;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductByIdForManage;

public sealed class GetProductByIdForManageValidator : AbstractValidator<GetProductByIdForManageQuery>
{
    public GetProductByIdForManageValidator(IMenuDbContext menuDbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID must not be empty.")
            .ChildRules(id =>
                id.RuleFor(id => id)
                    .MustAsync(async (id, ct) => await menuDbContext.Products.Exists(p => p.Id == (ProductId)id, ct))
                    .WithMessage("Product with the specified ID does not exist."));
    }
}
