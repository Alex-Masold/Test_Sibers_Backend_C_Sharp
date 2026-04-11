using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;
using Persistence.Extensions.Sort.Base;

namespace Persistence.Extensions.Sort;

internal static class ProjectOrderByExtensions
{
    public static IQueryable<Project> ApplyOrdering(
        this IQueryable<Project> query,
        SortOptions<ProjectSortField>? options
    )
    {
        return SortExtensionsBase.ApplyOrdering(query, options, e => e.Id, ApplyDirection);
    }

    private static IOrderedQueryable<Project> ApplyDirection(
        SortItem<ProjectSortField> sortItem,
        IOrderedQueryable<Project>? ordered,
        IQueryable<Project> query
    )
    {
        switch (sortItem.Field)
        {
            case ProjectSortField.Name:
                return OrderByHelper.ApplySort(ordered, query, e => e.Name, sortItem.Desc);
            case ProjectSortField.Priority:
                return OrderByHelper.ApplySort(ordered, query, e => e.Priority, sortItem.Desc);
            case ProjectSortField.CompanyOrderingName:
                return OrderByHelper.ApplySort(
                    ordered,
                    query,
                    e => e.CompanyOrdering,
                    sortItem.Desc
                );
            case ProjectSortField.CompanyExecutingName:
                return OrderByHelper.ApplySort(
                    ordered,
                    query,
                    e => e.CompanyExecuting,
                    sortItem.Desc
                );
            case ProjectSortField.StartDate:
                return OrderByHelper.ApplySort(ordered, query, e => e.StartDate, sortItem.Desc);
            case ProjectSortField.EndDate:
                return OrderByHelper.ApplySort(ordered, query, e => e.EndDate, sortItem.Desc);
            case ProjectSortField.TaskTotalCount:
                return OrderByHelper.ApplySort(ordered, query, e => e.Tasks.Count, sortItem.Desc);
            case ProjectSortField.MembersTotalCount:
                return OrderByHelper.ApplySort(ordered, query, e => e.Members.Count, sortItem.Desc);
            default:
                return ordered ?? query.OrderBy(e => e.Id);
        }
    }
}
