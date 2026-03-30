using Domain.Filters;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;

namespace Persistence.Extensions.Filters;

public static class EmployeeFilteringExtensions
{
    public static IQueryable<Employee> ApplyFilter(
        this IQueryable<Employee> query,
        EmployeeFilter? filter
    )
    {
        if (filter == null)
            return query;

        if (filter.Role.HasValue)
            query = query.Where(e => e.Role == filter.Role.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
        {
            var search = QueryHelpers.NormalizeSearch(filter.SearchQuery);
            query = query.Where(e =>
                EF.Functions.Collate(e.FirstName, "NOCASE").Contains(search)
                || EF.Functions.Collate(e.Email, "NOCASE").Contains(search)
                || (
                    e.MiddleName != null
                    && EF.Functions.Collate(e.MiddleName, "NOCASE").Contains(search)
                )
                || EF.Functions.Collate(e.LastName, "NOCASE").Contains(search)
            );
        }
        
        if (!string.IsNullOrWhiteSpace(filter.FirstName))
        {
            var normalize = QueryHelpers.NormalizeSearch(filter.FirstName);
            query = query.Where(e => e.FirstName.ToLower().Trim().Contains(normalize));
        }

        if (!string.IsNullOrWhiteSpace(filter.MiddleName))
        {
            var normalize = QueryHelpers.NormalizeSearch(filter.MiddleName);
            query = query.Where(e =>
                e.MiddleName != null
                && EF.Functions.Collate(e.MiddleName, "NOCASE").Contains(normalize)
            );
        }

        if (!string.IsNullOrWhiteSpace(filter.LastName))
        {
            var normalize = QueryHelpers.NormalizeSearch(filter.LastName);
            query = query.Where(e =>
                EF.Functions.Collate(e.LastName, "NOCASE").Contains(normalize)
            );
        }

        if (!string.IsNullOrWhiteSpace(filter.Email))
        {
            var normalize = QueryHelpers.NormalizeSearch(filter.Email);
            query = query.Where(e => EF.Functions.Collate(e.Email, "NOCASE").Contains(normalize));
        }

        if (filter.RelatedProjectId.HasValue)
        {
            var projectId = filter.RelatedProjectId.Value;
            query = query.Where(e => e.Memberships.Any(m => m.ProjectId == projectId));
        }

        return query;
    }
}
