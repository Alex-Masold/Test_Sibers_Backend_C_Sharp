using System.Linq.Expressions;
using Domain.Filters;
using Domain.Models;
using Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Persistence.DataContext;
using Persistence.ExpressionBuilders;
using Persistence.Extensions.Filters;
using Persistence.Extensions.Helpers;

namespace Persistence.Repositories;

public class ProjectMemberRepository(ApplicationContext context) : IProjectMemberStore
{
    public async Task<bool> MemberExistsAsync(
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

    public async Task<IReadOnlyCollection<(int ProjectId, int EmployeeId)>> MembersExistsAsync(
        IReadOnlyCollection<(int ProjectId, int EmployeeId)> pairs,
        CancellationToken cancellationToken = default
    )
    {
        if (pairs.Count == 0)
            return [];

        var filter = ProjectMemberExpressionBuilder.BuildKeyPair(pairs);

        var existing = await context
            .ProjectMembers.Where(filter)
            .Select(pm => new { pm.ProjectId, pm.EmployeeId })
            .ToListAsync(cancellationToken);

        return existing.Select(e => (e.ProjectId, e.EmployeeId)).ToList();
    }

    /// <summary>
    /// return the tracked object for updating via UnitOfWork.
    /// </summary>
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
        IReadOnlyCollection<(int ProjectId, int EmployeeId)> pairs,
        CancellationToken cancellationToken = default
    )
    {
        if (pairs == null || pairs.Count == 0)
            return new List<ProjectMember>();

        var filter = ProjectMemberExpressionBuilder.BuildKeyPair(pairs);

        var members = await context
            .ProjectMembers.Include(pm => pm.Project)
            .Include(pm => pm.Employee)
            .Where(filter)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return members;
    }

    public async Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetPagedAsync<T>(
        int pageNumber,
        int pageSize,
        Expression<Func<ProjectMember, T>> projection,
        ProjectMemberFilter? filter = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.ProjectMembers.AsNoTracking().ApplyFilter(filter);

        var result = await query
            .OrderBy(pm => pm.ProjectId)
            .ThenBy(pm => pm.EmployeeId)
            .ToPagedListAsync(pageNumber, pageSize, projection, cancellationToken);

        return result;
    }

    public ProjectMember Create(ProjectMember member)
    {
        var createdMember = context.ProjectMembers.Add(member);
        return createdMember.Entity;
    }

    public void CreateRange(IReadOnlyCollection<ProjectMember> projectMembers)
    {
        context.ProjectMembers.AddRange(projectMembers);
    }

    public async Task<int> DeleteAsync(
        int projectId,
        int employeeId,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .ProjectMembers.Where(pm => pm.ProjectId == projectId && pm.EmployeeId == employeeId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int> DeleteAsync(
        IReadOnlyCollection<(int ProjectId, int EmployeeId)> pairs,
        CancellationToken cancellationToken = default
    )
    {
        if (pairs.Count == 0)
            return 0;

        var filter = ProjectMemberExpressionBuilder.BuildKeyPair(pairs);

        return await context.ProjectMembers.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }
}
