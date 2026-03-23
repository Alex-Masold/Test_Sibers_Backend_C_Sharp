using Domain.Filters;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;

namespace Persistence.Extensions.Filters;

public static class ProjectFilteringExtensions
{
    public static IQueryable<Project> ApplyFilter(
        this IQueryable<Project> query,
        ProjectFilter? filter
    )
    {
        if (filter == null)
            return query;

        if (filter.Priority != null)
        {
            if (filter.Priority.Min.HasValue)
                query = query.Where(p => p.Priority >= filter.Priority.Min.Value);
            if (filter.Priority.Max.HasValue)
                query = query.Where(p => p.Priority <= filter.Priority.Max.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var normalize = QueryHelpers.NormalizeSearch(filter.Name);
            query = query.Where(p => EF.Functions.Collate(p.Name, "NOCASE").Contains(normalize));
        }

        if (!string.IsNullOrWhiteSpace(filter.CompanyOrdering))
        {
            var normalize = QueryHelpers.NormalizeSearch(filter.CompanyOrdering);
            query = query.Where(p =>
                EF.Functions.Collate(p.CompanyOrdering, "NOCASE").Contains(normalize)
            );
        }

        if (!string.IsNullOrWhiteSpace(filter.CompanyExecuting))
        {
            var normalize = QueryHelpers.NormalizeSearch(filter.CompanyExecuting);
            query = query.Where(p =>
                p.CompanyExecuting != null
                && EF.Functions.Collate(p.CompanyExecuting, "NOCASE").Contains(normalize)
            );
        }

        if (filter.StartDate != null)
        {
            if (filter.StartDate.Min.HasValue)
                query = query.Where(p => p.StartDate >= filter.StartDate.Min.Value);
            if (filter.StartDate.Max.HasValue)
                query = query.Where(p => p.StartDate <= filter.StartDate.Max.Value);
        }

        if (filter.EndDate != null)
        {
            if (filter.EndDate.Min.HasValue)
                query = query.Where(p => p.EndDate >= filter.EndDate.Min.Value);
            if (filter.EndDate.Max.HasValue)
                query = query.Where(p => p.EndDate <= filter.EndDate.Max.Value);
        }

        if (filter.ManagerId.HasValue)
            query = query.Where(p => p.ManagerId == filter.ManagerId.Value);

        if (filter.RelatedEmployeeId.HasValue)
        {
            var employeeId = filter.RelatedEmployeeId.Value;
            query = query.Where(p =>
                p.ManagerId == employeeId || p.Members.Any(m => m.EmployeeId == employeeId)
            );
        }

        return query;
    }
}
