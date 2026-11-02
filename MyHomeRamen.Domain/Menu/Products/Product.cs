using System.Collections.ObjectModel;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Common.Product;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Domain.Menu.Products;

public sealed class Product : AuditableEntity, IEntity<ProductId>
{
    private readonly Collection<Ingredient> _baseIngredients = [];
    private readonly Collection<Ingredient> _customIngredients = [];
    private readonly Collection<Category> _categories = [];

    public ProductId Id { get; private set; }

    public Guid CompanyId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string ImageUrl { get; private set; } = string.Empty;

    public ProductStatus Status { get; private set; }

    public IReadOnlyList<Ingredient> BaseIngredients => _baseIngredients.ToList();

    public IReadOnlyList<Ingredient> CustomIngredients => _customIngredients.ToList();

    public IReadOnlyList<Category> Categories => _categories.ToList();

    private Product()
    {
    }

    private Product(ProductId id, Guid companyId, Collection<Ingredient> baseIngredients, Collection<Ingredient> customIngredients, Collection<Category> categories)
    {
        Id = id;
        CompanyId = companyId;
        _baseIngredients = baseIngredients;
        _customIngredients = customIngredients;
        _categories = categories;
    }

    public static Product Create(ProductId id, Guid companyId, string name, string description, string imageUrl, Collection<Ingredient> baseIngredients, Collection<Ingredient> customIngredients, Collection<Category> categories)
    {
        Product product = new(id, companyId, baseIngredients, customIngredients, categories)
        {
            Name = name,
            Description = description,
            ImageUrl = imageUrl,
            Status = ProductStatus.Active
        };

        ProductValidator.ValidateProduct(product);

        return product;
    }

    public void Update(string name, string description, Category category, IEnumerable<Ingredient> ingredients, IEnumerable<Ingredient> customIngredients)
    {
        if (category is null)
        {
            throw ProductErrors.CategoryRequired();
        }

        Name = name;
        Description = description;

        _categories.Clear();
        _categories.Add(category);

        _baseIngredients.Clear();
        foreach (Ingredient ingredient in ingredients)
        {
            _baseIngredients.Add(ingredient);
        }

        _customIngredients.Clear();
        foreach (Ingredient ingredient in customIngredients)
        {
            _customIngredients.Add(ingredient);
        }

        ProductValidator.ValidateProduct(this);
    }

    public void Retire() => Status = ProductStatus.Retired;
}
