using System.Linq.Expressions;
using Domain.Sort.Base;
using Persistence.DataContext;

namespace Persistence.Extensions.Sort.Base;

internal static class SortExtensionsBase
{
    public static IQueryable<TEntity> ApplyOrdering<TEntity, TField, TDefault>(
        IQueryable<TEntity> query,
        SortOptions<TField>? options,
        Expression<Func<TEntity, TDefault>> defaultSort,
        Func<
            SortItem<TField>,
            IOrderedQueryable<TEntity>?,
            IQueryable<TEntity>,
            IOrderedQueryable<TEntity>
        > applyDirection
    )
        where TField : struct, Enum
    {
        if (options == null || options.Items.Count == 0)
            return query.OrderBy(defaultSort);

        IOrderedQueryable<TEntity>? ordered = null;
        foreach (var item in options.Items)
            ordered = applyDirection(item, ordered, query);

        return ordered!.ThenBy(defaultSort);
    }
}
