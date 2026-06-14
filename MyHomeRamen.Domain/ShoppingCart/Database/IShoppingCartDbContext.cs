using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Domain.ShoppingCart.Database;

public interface IShoppingCartDbContext : IBaseDbContext
{
    DbSet<Basket> ShoppingCarts { get; }

    DbSet<BasketItem> BasketItems { get; }

    DbSet<Product> Products { get; }

    DbSet<Ingredient> Ingredients { get; }

    DbSet<User> Users { get; }

    DbSet<Role> Roles { get; }

    DbSet<Permission> Permissions { get; }
    
    DbSet<PaymentDetails.PaymentDetails> PaymentDetails { get; }
    
    DbSet<ShippingDetails.ShippingDetails> ShippingDetails { get; }
}
