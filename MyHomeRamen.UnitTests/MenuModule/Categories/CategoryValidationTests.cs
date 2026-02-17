using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Category;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.UnitTests.MenuModule.Categories;

public class CategoryValidationTests
{
    private static readonly CategoryId DefaultId = new(Guid.NewGuid());
    private static readonly Guid DefaultRestaurantId = Guid.NewGuid();
    private const string DefaultName = "Soups";
    private const int DefaultSortOrder = 1;
    private const CategoryType DefaultCategoryType = CategoryType.Product;

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Act
        Category category = Category.Create(DefaultId, DefaultRestaurantId, DefaultName, DefaultSortOrder, DefaultCategoryType);

        // Assert
        Assert.Equal(DefaultId, category.Id);
        Assert.Equal(DefaultName, category.Name);
        Assert.Equal(DefaultSortOrder, category.SortOrder);
        Assert.Equal(DefaultCategoryType, category.CategoryType);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooShort()
    {
        // Arrange
        string name = new('a', CategoryConstants.MinNameLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateCategory(name: name));
        Assert.Equal(CategoryErrors.NameTooShort().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooLong()
    {
        // Arrange
        string name = new('a', CategoryConstants.MaxNameLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateCategory(name: name));
        Assert.Equal(CategoryErrors.NameTooLong().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_SortOrderIsTooSmall()
    {
        // Arrange
        int sortOrder = CategoryConstants.MinSortOrder - 1;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateCategory(sortOrder: sortOrder));
        Assert.Equal(CategoryErrors.SortOrderTooSmall().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_CategoryTypeIsInvalid()
    {
        // Arrange
        CategoryType categoryType = (CategoryType)999;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateCategory(categoryType: categoryType));
        Assert.Equal(CategoryErrors.CategoryTypeInvalid().Message, exception.Message);
    }

    private static Category CreateCategory(
        string? name = null,
        int? sortOrder = null,
        CategoryType? categoryType = null)
    {
        return Category.Create(
            DefaultId,
            DefaultRestaurantId,
            name ?? DefaultName,
            sortOrder ?? DefaultSortOrder,
            categoryType ?? DefaultCategoryType);
    }
}
