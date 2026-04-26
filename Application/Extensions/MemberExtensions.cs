using Domain.Exceptions;
using Domain.Models;
using Domain.Stores;

namespace Application.Extensions;

public static class MemberExtensions
{
    public static async Task<ProjectMember> GetOrThrowAsync(
        this IProjectMemberStore memberStore,
        int projectId,
        int employeeId,
        CancellationToken ct = default
    )
    {
        var member = await memberStore.GetByIdAsync(projectId, employeeId, ct);
        if (member is null)
            throw new NotFoundException(nameof(ProjectMember), (projectId, employeeId));
        return member;
    }

    public static async Task<IReadOnlyCollection<ProjectMember>> GetOrThrowAsync(
        this IProjectMemberStore memberStore,
        IReadOnlyCollection<(int ProjectId, int EmployeeId)> pairs,
        CancellationToken ct = default
    )
    {
        var distinctPairs = pairs.Distinct().ToList();
        var existingMembers = await memberStore.GetRangeByIdsAsync(distinctPairs, ct);

        if (existingMembers.Count != distinctPairs.Count)
        {
            var existingId = existingMembers
                .Select(pm => (pm.ProjectId, pm.EmployeeId))
                .ToHashSet();
            var missing = distinctPairs
                .Where(p => !existingId.Contains(p))
                .Select(p => (object)$"({p.ProjectId},{p.EmployeeId})")
                .ToList();
            throw new NotFoundException(nameof(ProjectMember), missing);
        }

        return existingMembers;
    }
}
