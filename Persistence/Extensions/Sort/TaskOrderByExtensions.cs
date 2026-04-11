using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;
using Persistence.Extensions.Sort.Base;

namespace Persistence.Extensions.Sort;

internal static class TaskOrderByExtensions
{
    public static IQueryable<WorkTask> ApplyOrdering(
        this IQueryable<WorkTask> query,
        SortOptions<TaskSortField>? options
    )
    {
        return SortExtensionsBase.ApplyOrdering(query, options, e => e.Id, ApplyDirection);
    }

    private static IOrderedQueryable<WorkTask> ApplyDirection(
        SortItem<TaskSortField> sortItem,
        IOrderedQueryable<WorkTask>? ordered,
        IQueryable<WorkTask> query
    )
    {
        switch (sortItem.Field)
        {
            case TaskSortField.Title:
                return OrderByHelper.ApplySort(ordered, query, t => t.Title, sortItem.Desc);
            case TaskSortField.Priority:
                return OrderByHelper.ApplySort(ordered, query, t => t.Priority, sortItem.Desc);
            case TaskSortField.Status:
                return OrderByHelper.ApplySort(ordered, query, t => t.Status, sortItem.Desc);
            case TaskSortField.Comment:
                return OrderByHelper.ApplySort(ordered, query, t => t.Comment, sortItem.Desc);
            case TaskSortField.CreatedAt:
                return OrderByHelper.ApplySort(ordered, query, t => t.CreatedAt, sortItem.Desc);
            case TaskSortField.UpdatedAt:
                return OrderByHelper.ApplySort(ordered, query, t => t.UpdatedAt, sortItem.Desc);
            case TaskSortField.Author:
                return OrderByHelper.ApplySort(
                    ordered,
                    query,
                    t => t.Author != null ? t.Author.LastName : null,
                    sortItem.Desc
                );
            case TaskSortField.Executor:
                return OrderByHelper.ApplySort(
                    ordered,
                    query,
                    t => t.Executor != null ? t.Executor.LastName : null,
                    sortItem.Desc
                );
            case TaskSortField.Project:
                return OrderByHelper.ApplySort(ordered, query, t => t.Project.Name, sortItem.Desc);
            default:
                return ordered ?? query.OrderBy(t => t.Id);
        }
    }
}
