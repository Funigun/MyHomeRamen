using FluentValidation;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;

namespace MyHomeRamen.Features.Menu.Features.Categories.CreateCategory;

public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Request.Name)
            .MustMeetLengthRequirements()
            .MustHaveUniqueName(dbContext);

        RuleFor(x => x.Request.CategoryType)
            .MustBeValidCategoryType();
    }
}
