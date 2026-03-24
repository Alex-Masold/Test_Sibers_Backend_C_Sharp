using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;
using Persistence.Extensions.Sort.Base;

namespace Persistence.Extensions.Sort;

public static class EmployeeOrderByExtensions
{
    public static IQueryable<Employee> ApplyOrdering(
        this IQueryable<Employee> query,
        SortOptions<EmployeeSortField>? options
    )
    {
        if (options == null || options.Items.Count == 0)
            return query.OrderBy(e => e.Id);

        IOrderedQueryable<Employee>? ordered = null;

        foreach (var item in options.Items)
        {
            ordered = ApplyDirection(item, ordered, query);
        }
        return ordered!.ThenBy(e => e.Id);
    }

    private static IOrderedQueryable<Employee> ApplyDirection(
        SortItem<EmployeeSortField> sortItem,
        IOrderedQueryable<Employee>? ordered,
        IQueryable<Employee> query
    )
    {
        switch (sortItem.Field)
        {
            case EmployeeSortField.FirstName:
                return OrderByHelper.ApplySort(ordered, query, e => e.FirstName, sortItem.Desc);
            case EmployeeSortField.MiddleName:
                return OrderByHelper.ApplySort(ordered, query, e => e.MiddleName, sortItem.Desc);
            case EmployeeSortField.LastName:
                return OrderByHelper.ApplySort(ordered, query, e => e.LastName, sortItem.Desc);
            case EmployeeSortField.Email:
                return OrderByHelper.ApplySort(ordered, query, e => e.Email, sortItem.Desc);
            case EmployeeSortField.Role:
                return OrderByHelper.ApplySort(ordered, query, e => e.Role, sortItem.Desc);
            case EmployeeSortField.TaskTotalCount:
                return OrderByHelper.ApplySort(
                    ordered,
                    query,
                    e => e.AssignedTasks.Count,
                    sortItem.Desc
                );
            default:
                return ordered ?? query.OrderBy(e => e.Id);
        }
    }
}
