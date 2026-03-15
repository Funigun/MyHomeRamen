using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Persistance.Common;

public static class DbExtensions
{
    public static async Task<bool> ExistsByIdAsync<TEntity, TId>(
        this IQueryable<TEntity> query,
        TId id,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TId>
        where TId : IEntityId
    {
        ParameterExpression? parameter = Expression.Parameter(typeof(TEntity), "e");
        MemberExpression? property = Expression.Property(parameter, nameof(IEntity<TId>.Id));
        ConstantExpression? constant = Expression.Constant(id, typeof(TId));
        BinaryExpression? body = Expression.Equal(property, constant);
        Expression<Func<TEntity, bool>>? lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return await query.AnyAsync(lambda, cancellationToken);
    }

    public static async Task<bool> IsNameUniqueAsync(
        this IQueryable<MyHomeRamen.Domain.Menu.Products.Product> query,
        string name,
        CancellationToken cancellationToken = default)
    {
        return !await query.AnyAsync(p => p.Name.ToLower() == name.ToLower(), cancellationToken);
    }
}
