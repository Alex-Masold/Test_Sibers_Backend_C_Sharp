using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;

namespace Persistence.Extensions.Sort;

public static class  ProjectOrderByExtensionsReflection
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
    
    public static IQueryable<Project> ApplyOrderingReflection(
        this IQueryable<Project> query,
        SortOptions<ProjectSortField>? options
    )
    {
        if (options == null || options.Items.Count == 0)
            return query.OrderBy(e => e.Id);

        IOrderedQueryable<Project>? ordered = null;

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

    private static readonly Dictionary<ProjectSortField, LambdaExpression> _selectors = new()
    {
        { ProjectSortField.Name, (Expression<Func<Project, string>>)(e => e.Name) },
        { ProjectSortField.Priority, (Expression<Func<Project, int>>)(e => e.Priority) },
        {
            ProjectSortField.CompanyOrderingName,
            (Expression<Func<Project, string>>)(e => e.CompanyOrdering)
        },
        {
            ProjectSortField.CompanyExecutingName,
            (Expression<Func<Project, string?>>)(e => e.CompanyExecuting)
        },
        { ProjectSortField.StartDate, (Expression<Func<Project, DateOnly>>)(e => e.StartDate) },
        { ProjectSortField.EndDate, (Expression<Func<Project, DateOnly?>>)(e => e.EndDate) },
        { ProjectSortField.TaskTotalCount, (Expression<Func<Project, int>>)(e => e.Tasks.Count) },
        {
            ProjectSortField.MembersTotalCount,
            (Expression<Func<Project, int>>)(e => e.Members.Count)
        },
    };

    private static LambdaExpression GetSelector(ProjectSortField field) =>
        _selectors.TryGetValue(field, out var selector)
            ? selector
            : (Expression<Func<Project, object>>)(e => e.Id);

    private static IOrderedQueryable<Project> ApplyDirection(
        IQueryable<Project> source,
        LambdaExpression keySelector,
        bool desc,
        bool isFirst
    )
    {
        var methodName = isFirst
            ? (desc ? "OrderByDescending" : "OrderBy")
            : (desc ? "ThenByDescending" : "ThenBy");

        var method = GetMethod(methodName, keySelector.ReturnType);

        return (IOrderedQueryable<Project>)
            method.Invoke(null, new object[] { source, keySelector })!;
    }
}
