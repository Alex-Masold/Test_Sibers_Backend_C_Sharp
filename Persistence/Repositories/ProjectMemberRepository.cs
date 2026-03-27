using System.Linq.Expressions;
using Domain.Filters;
using Domain.Models;
using Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Persistence.DataContext;
using Persistence.Extensions.Filters;

namespace Persistence.Repositories;

public class ProjectMemberRepository(ApplicationContext context) : IProjectMemberStore
{
    public async Task LoadProjectAsync(
        ProjectMember member,
        CancellationToken cancellationToken = default
    )
    {
        await context.Entry(member).Reference(m => m.Project).LoadAsync(cancellationToken);
    }

    public async Task LoadEmployeeAsync(
        ProjectMember member,
        CancellationToken cancellationToken = default
    )
    {
        await context.Entry(member).Reference(m => m.Employee).LoadAsync(cancellationToken);
    }

    public async Task<bool> MemberExistAsync(
        int projectId,
        int employeeId,
        CancellationToken cancellationToken = default
    )
    {
        return await context.ProjectMembers.AnyAsync(
            pm => pm.ProjectId == projectId && pm.EmployeeId == employeeId,
            cancellationToken
        );
    }

    public async Task<IReadOnlyCollection<(int ProjectId, int EmployeeId)>> MemberExistAsync(
        IReadOnlyCollection<(int ProjectId, int EmployeeId)> pairs,
        CancellationToken cancellationToken = default
    )
    {
        var projectIds = pairs.Select(p => p.ProjectId).Distinct();
        var employeeIds = pairs.Select(e => e.EmployeeId).Distinct();

        var existing = await context
            .ProjectMembers.Where(pm =>
                projectIds.Contains(pm.ProjectId) && employeeIds.Contains(pm.EmployeeId)
            )
            .Select(pm => new { pm.ProjectId, pm.EmployeeId })
            .ToListAsync(cancellationToken);

        var result = existing
            .Where(e =>
                pairs.Any(pm => pm.ProjectId == e.ProjectId && pm.EmployeeId == e.EmployeeId)
            )
            .Select(e => (e.ProjectId, e.EmployeeId))
            .ToList();

        return result;
    }

    public async Task<ProjectMember?> GetByIdAsync(
        int projectId,
        int employeeId,
        CancellationToken cancellationToken = default
    )
    {
        var member = await context
            .ProjectMembers.Include(pm => pm.Project)
            .Include(pm => pm.Employee)
            .FirstOrDefaultAsync(
                pm => pm.ProjectId == projectId && pm.EmployeeId == employeeId,
                cancellationToken
            );

        return member;
    }

    public async Task<IReadOnlyCollection<ProjectMember>> GetRangeByIdsAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    )
    {
        if (idList == null || idList.Count == 0)
            return new List<ProjectMember>();

        var members = await context
            .ProjectMembers.Include(pm => pm.Project)
            .Include(pm => pm.Employee)
            .Where(pm => idList.Contains(pm.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return members;
    }

    public async Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetPagedAsync<T>(
        int pageNumber,
        int pageSize,
        Expression<Func<ProjectMember, T>> projection,
        ProjectMemberFilter? filter = null,
        CancellationToken ct = default
    )
    {
        var query = context.ProjectMembers.AsNoTracking().ApplyFilter(filter);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(pm => pm.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(projection)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public ProjectMember Create(ProjectMember member)
    {
        var createdmember = context.ProjectMembers.Add(member);
        return createdmember.Entity;
    }

    public void CreateRange(IReadOnlyCollection<ProjectMember> projectMembers)
    {
        context.ProjectMembers.AddRange(projectMembers);
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context
            .ProjectMembers.Where(pm => pm.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int> DeleteAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .ProjectMembers.Where(pm => idList.Contains(pm.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
