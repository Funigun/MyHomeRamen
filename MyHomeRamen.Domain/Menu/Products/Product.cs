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

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public string ImageUrl { get; private set; } = string.Empty;

    public IReadOnlyList<Ingredient> BaseIngredients => _baseIngredients.ToList();

    public IReadOnlyList<Ingredient> CustomIngredients => _customIngredients.ToList();

    public IReadOnlyList<Category> Categories => _categories.ToList();

    private Product()
    {
    }

    private Product(ProductId id, Collection<Ingredient> baseIngredients, Collection<Ingredient> customIngredients, Collection<Category> categories)
    {
        Id = id;
        _baseIngredients = baseIngredients;
        _customIngredients = customIngredients;
        _categories = categories;
    }

    public static Product Create(ProductId id, string name, string description, decimal price, string imageUrl, Collection<Ingredient> baseIngredients, Collection<Ingredient> customIngredients, Collection<Category> categories)
    {
        Product product = new(id, baseIngredients, customIngredients, categories)
        {
            Name = name,
            Description = description,
            Price = price,
            ImageUrl = imageUrl
        };

        ProductValidator.ValidateProduct(product);

        return product;
    }

    public void Update(string name, string description, decimal price, Category category, IEnumerable<Ingredient> ingredients, IEnumerable<Ingredient> customIngredients)
    {
        if (category is null)
        {
            throw ProductErrors.CategoryRequired();
        }

        Name = name;
        Description = description;
        Price = price;

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
}
