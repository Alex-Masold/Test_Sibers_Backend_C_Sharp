using System.Linq.Expressions;
using Domain.Filters;
using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;

namespace Domain.Stores;

public interface IProjectStore
{
    Task<Project?> GetByIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Project>> GetRangeByIdsAsync(
        IReadOnlyCollection<int> projectIdList,
        CancellationToken cancellationToken = default
    );
    Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetPagedAsync<T>(
        int pageNumber,
        int pageSize,
        Expression<Func<Project, T>> projection,
        ProjectFilter? filter = null,
        SortOptions<ProjectSortField>? options = null,
        CancellationToken cancellationToken = default
    );
    Project Create(Project project);
    Task<int> DeleteAsync(int projectId, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(
        IReadOnlyCollection<int> projectIdList,
        CancellationToken cancellationToken = default
    );
}
