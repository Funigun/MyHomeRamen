using FluentValidation;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Products.Common;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductByIdForManage;

public sealed class GetProductByIdForManageValidator : AbstractValidator<GetProductByIdForManageQuery>
{
    public GetProductByIdForManageValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .MustBeValidProductId(dbContext);
    }
}
