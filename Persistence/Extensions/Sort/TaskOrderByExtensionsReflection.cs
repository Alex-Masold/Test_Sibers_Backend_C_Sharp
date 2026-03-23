using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;

namespace Persistence.Extensions.Sort;

public static class TaskOrderByExtensionsReflection
{
    private static readonly ConcurrentDictionary<string, MethodInfo> _cache = new();

    private static MethodInfo GetMethod(string methodName, Type keyType)
    {
        var cacheKey = $"{methodName}_{keyType.Name}";

        return _cache.GetOrAdd(
            cacheKey,
            _ =>
            {
                var method = typeof(Queryable)
                    .GetMethods()
                    .Single(m => m.Name == methodName && m.GetParameters().Length == 2);
                return method.MakeGenericMethod(typeof(Employee), keyType);
            }
        );
    }
    
    public static IQueryable<WorkTask> ApplyOrderingReflection(
        this IQueryable<WorkTask> query,
        SortOptions<TaskSortField>? options
    )
    {
        if (options == null || options.Items.Count == 0)
            return query.OrderBy(e => e.Id);

        IOrderedQueryable<WorkTask>? ordered = null;

        foreach (var item in options.Items)
        {
            var keySelector = GetSelector(item.Field);
            ordered = ApplyDirection(
                ordered ?? query,
                keySelector,
                item.Desc,
                isFirst: ordered == null
            );
        }

        return ordered!.ThenBy(e => e.Id);
    }

    private static readonly Dictionary<TaskSortField, LambdaExpression> _selectors = new()
    {
        { TaskSortField.Title, (Expression<Func<WorkTask, string>>)(t => t.Title) },
        { TaskSortField.Priority, (Expression<Func<WorkTask, int>>)(t => t.Priority) },
        { TaskSortField.Status, (Expression<Func<WorkTask, int>>)(t => (int)t.Status) },
        { TaskSortField.Comment, (Expression<Func<WorkTask, string?>>)(t => t.Comment) },
        { TaskSortField.CreatedAt, (Expression<Func<WorkTask, DateTime>>)(t => t.CreatedAt) },
        { TaskSortField.UpdatedAt, (Expression<Func<WorkTask, DateTime?>>)(t => t.UpdatedAt) },
        {
            TaskSortField.Author,
            (Expression<Func<WorkTask, string?>>)(t => t.Author != null ? t.Author.LastName : null)
        },
        {
            TaskSortField.Executor,
            (Expression<Func<WorkTask, string?>>)(
                t => t.Executor != null ? t.Executor.LastName : null
            )
        },
        { TaskSortField.Project, (Expression<Func<WorkTask, string>>)(t => t.Project.Name) },
    };

    private static LambdaExpression GetSelector(TaskSortField field) =>
        _selectors.TryGetValue(field, out var selector)
            ? selector
            : (Expression<Func<WorkTask, object>>)(e => e.Id);

    private static IOrderedQueryable<WorkTask> ApplyDirection(
        IQueryable<WorkTask> source,
        LambdaExpression keySelector,
        bool desc,
        bool isFirst
    )
    {
        var methodName = isFirst
            ? (desc ? "OrderByDescending" : "OrderBy")
            : (desc ? "ThenByDescending" : "ThenBy");

        var method = GetMethod(methodName, keySelector.ReturnType);
        
        return (IOrderedQueryable<WorkTask>)
            method.Invoke(null, new object[] { source, keySelector })!;
    }
}
