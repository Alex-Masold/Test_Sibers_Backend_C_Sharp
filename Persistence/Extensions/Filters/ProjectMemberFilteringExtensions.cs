using Domain.Filters;
using Domain.Models;

namespace Persistence.Extensions.Filters;

public static class ProjectMemberFilteringExtensions
{
    public static IQueryable<ProjectMember> ApplyFilter(
        this IQueryable<ProjectMember> query,
        ProjectMemberFilter? filter
    )
    {
        if (filter == null)
            return query;

        if (filter.ProjectId.HasValue)
        {
            query = query.Where(pm => pm.ProjectId == filter.ProjectId.Value);
        }

        if (filter.EmployeeId.HasValue)
        {
            query = query.Where(pm => pm.EmployeeId == filter.EmployeeId.Value);
        }

        return query;
    }
}
