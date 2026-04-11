using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;
using Persistence.Extensions.Sort.Base;

namespace Persistence.Extensions.Sort;

internal static class EmployeeOrderByExtensions
{
    public static IQueryable<Employee> ApplyOrdering(
        this IQueryable<Employee> query,
        SortOptions<EmployeeSortField>? options
    )
    {
        return SortExtensionsBase.ApplyOrdering(query, options, e => e.Id, ApplyDirection);
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
