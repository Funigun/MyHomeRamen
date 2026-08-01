using FluentValidation;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Products.Common;

namespace MyHomeRamen.Features.Menu.Features.Products.UpdateProduct;

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .MustBeValidProductId(dbContext);

        RuleFor(x => x.UpdateProductRequest.Name)
            .MustMeetProductNameLengthRequirements();

        RuleFor(x => x.UpdateProductRequest.Description)
            .MustMeetProductDescriptionLengthRequirements();

        RuleFor(x => x.UpdateProductRequest.Price)
            .MustBeValidProductPrice();

        RuleFor(x => x)
            .MustHaveUniqueProductNameExcluding(dbContext, c => c.UpdateProductRequest.Name, c => c.Id)
            .OverridePropertyName(nameof(UpdateProductCommand.UpdateProductRequest) + "." + nameof(UpdateProductRequest.Name));

        RuleFor(x => x.UpdateProductRequest.CategoryId)
            .MustBeExistingProductCategory(dbContext);

        RuleFor(x => x.UpdateProductRequest.IngredientIds)
            .MustContainIngredients();

        RuleFor(x => x.UpdateProductRequest.CustomIngredientIds)
            .MustContainExistingCustomIngredients(dbContext);

        RuleFor(x => x)
            .MustHaveDistinctIngredientIds(
                c => c.UpdateProductRequest.IngredientIds,
                c => c.UpdateProductRequest.CustomIngredientIds)
            .WithMessage("Ingredient IDs and custom ingredient IDs must be unique across both collections.");
    }
}
