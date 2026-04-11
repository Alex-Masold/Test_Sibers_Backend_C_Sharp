using Domain.Filters;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Base;
using Shared.Helpers;

namespace Persistence.Extensions.Filters;

internal static class EmployeeFilteringExtensions
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
                EF.Functions.Like(e.FirstName, search, DbConstants.Escape)
                || EF.Functions.Like(e.Email, search, DbConstants.Escape)
                || (
                    e.MiddleName != null
                    && EF.Functions.Like(e.MiddleName, search, DbConstants.Escape)
                )
                || EF.Functions.Like(e.LastName, search, DbConstants.Escape)
            );
        }

        if (!string.IsNullOrWhiteSpace(filter.FirstName))
        {
            var normalize = QueryHelpers.NormalizeSearch(filter.FirstName);
            query = query.Where(e => EF.Functions.Like(e.FirstName, normalize, DbConstants.Escape));
        }

        if (!string.IsNullOrWhiteSpace(filter.MiddleName))
        {
            var normalize = QueryHelpers.NormalizeSearch(filter.MiddleName);
            query = query.Where(e =>
                e.MiddleName != null
                && EF.Functions.Like(e.MiddleName, normalize, DbConstants.Escape)
            );
        }

        if (!string.IsNullOrWhiteSpace(filter.LastName))
        {
            var normalize = QueryHelpers.NormalizeSearch(filter.LastName);
            query = query.Where(e => EF.Functions.Like(e.LastName, normalize, DbConstants.Escape));
        }

        if (!string.IsNullOrWhiteSpace(filter.Email))
        {
            var normalize = QueryHelpers.NormalizeSearch(filter.Email);
            query = query.Where(e => EF.Functions.Like(e.Email, normalize, DbConstants.Escape));
        }

        if (filter.RelatedProjectId.HasValue)
        {
            var projectId = filter.RelatedProjectId.Value;
            query = query.Where(e => e.Memberships.Any(m => m.ProjectId == projectId));
        }

        return query;
    }
}
