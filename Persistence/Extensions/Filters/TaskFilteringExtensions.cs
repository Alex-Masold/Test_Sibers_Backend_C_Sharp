using Domain.Filters;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Base;
using Shared.Helpers;

namespace Persistence.Extensions.Filters;

internal static class TaskFilteringExtensions
{
    public static IQueryable<WorkTask> ApplyFilter(
        this IQueryable<WorkTask> query,
        TaskFilter? filter
    )
    {
        if (filter == null)
            return query;

        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            var normalized = QueryHelpers.NormalizeSearch(filter.Title);
            query = query.Where(t => EF.Functions.Like(t.Title, normalized, DbConstants.Escape));
        }
        if (!string.IsNullOrWhiteSpace(filter.Comment))
        {
            var normalized = QueryHelpers.NormalizeSearch(filter.Comment);
            query = query.Where(t =>
                t.Comment != null && EF.Functions.Like(t.Comment, normalized, DbConstants.Escape)
            );
        }

        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status);

        if (filter.AuthorId.HasValue)
            query = query.Where(t => t.AuthorId == filter.AuthorId.Value);
        if (filter.ExecutorId.HasValue)
            query = query.Where(t => t.ExecutorId == filter.ExecutorId.Value);
        if (filter.ProjectId.HasValue)
            query = query.Where(t => t.ProjectId == filter.ProjectId.Value);

        if (filter.Priority != null)
        {
            if (filter.Priority.Min.HasValue)
                query = query.Where(t => t.Priority >= filter.Priority.Min.Value);

            if (filter.Priority.Max.HasValue)
                query = query.Where(t => t.Priority <= filter.Priority.Max.Value);
        }
        if (filter.CreatedAt != null)
        {
            if (filter.CreatedAt.Min.HasValue)
                query = query.Where(t => t.CreatedAt >= filter.CreatedAt.Min.Value);

            if (filter.CreatedAt.Max.HasValue)
                query = query.Where(t => t.CreatedAt <= filter.CreatedAt.Max.Value);
        }
        if (filter.UpdatedAt != null)
        {
            if (filter.UpdatedAt.Min.HasValue)
                query = query.Where(t =>
                    t.UpdatedAt.HasValue && t.UpdatedAt.Value >= filter.UpdatedAt.Min.Value
                );

            if (filter.UpdatedAt.Max.HasValue)
                query = query.Where(t =>
                    t.UpdatedAt.HasValue && t.UpdatedAt.Value <= filter.UpdatedAt.Max.Value
                );
        }

        if (filter.ProjectManagerId.HasValue)
            query = query.Where(t => t.Project.ManagerId == filter.ProjectManagerId.Value);

        return query;
    }
}
