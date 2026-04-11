using System.Linq.Expressions;
using Domain.Filters;
using Domain.Models;

namespace Domain.Stores;

public interface IProjectMemberStore
{
    Task<bool> MemberExistsAsync(
        int projectId,
        int employeeId,
        CancellationToken cancellationToken = default
    );
    Task<IReadOnlyCollection<(int ProjectId, int EmployeeId)>> MembersExistsAsync(
        IReadOnlyCollection<(int ProjectId, int EmployeeId)> pairs,
        CancellationToken cancellationToken = default
    );
    Task<ProjectMember?> GetByIdAsync(
        int projectId,
        int employeeId,
        CancellationToken cancellationToken = default
    );
    Task<IReadOnlyCollection<ProjectMember>> GetRangeByIdsAsync(
        IReadOnlyCollection<(int ProjectId, int EmployeeId)> pairs,
        CancellationToken cancellationToken = default
    );

    Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetPagedAsync<T>(
        int pageNumber,
        int pageSize,
        Expression<Func<ProjectMember, T>> projection,
        ProjectMemberFilter? filter = null,
        CancellationToken cancellationToken = default
    );

    ProjectMember Create(ProjectMember projectMember);

    void CreateRange(IReadOnlyCollection<ProjectMember> projectMembers);

    Task<int> DeleteAsync(
        int projectId,
        int employeeId,
        CancellationToken cancellationToken = default
    );
    Task<int> DeleteAsync(
        IReadOnlyCollection<(int ProjectId, int EmployeeId)> pairs,
        CancellationToken cancellationToken = default
    );
}
