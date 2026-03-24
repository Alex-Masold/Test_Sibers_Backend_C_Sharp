using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;
using Persistence.Extensions.Sort.Base;

namespace Persistence.Extensions.Sort;

public static class TaskOrderByExtensions
{
    public static IQueryable<WorkTask> ApplyOrdering(
        this IQueryable<WorkTask> query,
        SortOptions<TaskSortField>? options
    )
    {
        if (options == null || options.Items.Count == 0)
            return query.OrderBy(e => e.Id);

        IOrderedQueryable<WorkTask>? ordered = null;

        foreach (var item in options.Items)
        {
            ordered = ApplyDirection(item, ordered, query);
        }
        return ordered!.ThenBy(e => e.Id);
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
