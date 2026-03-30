using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Extensions.Helpers;

internal static class QueryableExtensions
{
    public static async Task<(IReadOnlyCollection<T> Items, int TotalCount)> ToPagedListAsync<
        TEntity,
        T
    >(
        this IQueryable<TEntity> query,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, T>> projection,
        CancellationToken cancellationToken = default
    )
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(projection)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
