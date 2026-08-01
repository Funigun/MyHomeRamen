using FluentValidation;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Products.Common;

namespace MyHomeRamen.Features.Menu.Features.Products.CreateProduct;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.CreateProductRequest.Name)
            .MustMeetProductNameLengthRequirements()
            .MustHaveUniqueProductName(dbContext);

        RuleFor(x => x.CreateProductRequest.Description)
            .MustMeetProductDescriptionLengthRequirements();

        RuleFor(x => x.CreateProductRequest.Price)
            .MustBeValidProductPrice();

        RuleFor(x => x.CreateProductRequest.CategoryId)
            .MustBeExistingProductCategory(dbContext);

        RuleFor(x => x.CreateProductRequest.IngredientIds)
            .MustContainIngredients();

        RuleFor(x => x.CreateProductRequest.CustomIngredientIds)
            .MustContainExistingCustomIngredients(dbContext);

        RuleFor(x => x)
            .MustHaveDistinctIngredientIds(
                c => c.CreateProductRequest.IngredientIds,
                c => c.CreateProductRequest.CustomIngredientIds)
            .WithMessage("Ingredient IDs and custom ingredient IDs must be unique across both collections.");
    }
}
