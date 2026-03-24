using System.Linq.Expressions;

namespace Persistence.Extensions.Sort.Base;

internal static class OrderByHelper
{
    public static IOrderedQueryable<T> ApplySort<T, TKey>(
        IOrderedQueryable<T>? ordered,
        IQueryable<T> query,
        Expression<Func<T, TKey>> keySelector,
        bool desc
    )
    {
        if (ordered == null)
            return desc ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        return desc ? ordered.ThenByDescending(keySelector) : ordered.ThenBy(keySelector);
    }
}
