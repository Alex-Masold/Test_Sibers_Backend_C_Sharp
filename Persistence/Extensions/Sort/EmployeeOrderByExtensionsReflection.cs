using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;

namespace Persistence.Extensions.Sort;

public static class EmployeeOrderByExtensionsReflection
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

    public static IQueryable<Employee> ApplyOrderingReflection(
        this IQueryable<Employee> query,
        SortOptions<EmployeeSortField>? options
    )
    {
        if (options == null || options.Items.Count == 0)
            return query.OrderBy(e => e.Id);

        IOrderedQueryable<Employee>? ordered = null;

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

    private static readonly Dictionary<EmployeeSortField, LambdaExpression> _selectors = new()
    {
        { EmployeeSortField.FirstName, (Expression<Func<Employee, string>>)(e => e.FirstName) },
        { EmployeeSortField.MiddleName, (Expression<Func<Employee, string?>>)(e => e.MiddleName) },
        { EmployeeSortField.LastName, (Expression<Func<Employee, string>>)(e => e.LastName) },
        { EmployeeSortField.Email, (Expression<Func<Employee, string>>)(e => e.Email) },
        { EmployeeSortField.Role, (Expression<Func<Employee, Role>>)(e => e.Role) },
        {
            EmployeeSortField.TaskTotalCount,
            (Expression<Func<Employee, int>>)(e => e.AssignedTasks.Count)
        },
    };

    private static LambdaExpression GetSelector(EmployeeSortField field) =>
        _selectors.TryGetValue(field, out var selector)
            ? selector
            : (Expression<Func<Employee, object>>)(e => e.Id);

    private static IOrderedQueryable<Employee> ApplyDirection(
        IQueryable<Employee> source,
        LambdaExpression keySelector,
        bool desc,
        bool isFirst
    )
    {
        var methodName = isFirst
            ? (desc ? "OrderByDescending" : "OrderBy")
            : (desc ? "ThenByDescending" : "ThenBy");

        var method = GetMethod(methodName, keySelector.ReturnType);

        return (IOrderedQueryable<Employee>)
            method.Invoke(null, new object[] { source, keySelector })!;
    }
}
