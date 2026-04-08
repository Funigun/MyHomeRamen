using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage.Policies;

public sealed class GetProductByIdForManageValidator : AbstractValidator<GetProductByIdForManageRequest>
{
    public GetProductByIdForManageValidator(IMenuDbContext menuDbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID must not be empty.")
            .ChildRules(id =>
                id.RuleFor(id => id)
                    .MustAsync(async (id, ct) => await menuDbContext.Products.ExistsByIdAsync((ProductId)id, ct))
                    .WithMessage("Product with the specified ID does not exist."));
    }
}
