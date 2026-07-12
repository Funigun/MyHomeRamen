using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Products.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : IProductRepository
{
    public void Add(Product entity) 
        => Products.Add(entity);
    
    public void AddRange(IEnumerable<Product> entities) 
        => Products.AddRange(entities);

    async Task<int> IRepository<Product, ProductId>.Count(CancellationToken cancellationToken) 
        => await Products.CountAsync(cancellationToken);

    public void Delete(Product entity) 
        => Products.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken) 
        => await Products.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<Product, bool>> filterPredicate, Dictionary<Expression<Func<Product, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<Product>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await Products.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken) 
        => await Products.AsNoTracking().AnyAsync(predicate, cancellationToken);

    IProductQuery IProductRepository.Query() => this;

    IProductSpecification IProductRepository.Specification() => this;
}
