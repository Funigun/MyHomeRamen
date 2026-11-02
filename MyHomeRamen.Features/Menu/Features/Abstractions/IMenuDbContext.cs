using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Features.Menu.Features.Offers.Common;
using MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.Common;
using MyHomeRamen.Features.Menu.Features.MenuCards.Common;

namespace MyHomeRamen.Features.Menu.Features.Abstractions;

public interface IMenuDbContext : IUnitOfWork
{
    Guid CompanyId { get; }

    Guid RestaurantId { get; }

    IProductRepository Product { get; }

    ICategoryRepository Category { get; }

    IIngredientRepository Ingredient { get; }

    IRestaurantProductOfferRepository RestaurantProductOffer { get; }

    IRestaurantIngredientPriceRepository RestaurantIngredientPrice { get; }

    IMenuCardRepository MenuCard { get; }
}
