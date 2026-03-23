using System.Linq.Expressions;
using Domain.Filters;
using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;

namespace Domain.Stores;

public interface ITaskStore
{
    Task LoadAuthorAsync(WorkTask task, CancellationToken cancellationToken = default);
    Task LoadExecutorAsync(WorkTask task, CancellationToken cancellationToken = default);
    Task LoadProjectAsync(WorkTask task, CancellationToken cancellationToken = default);

    Task<WorkTask?> GetByIdAsync(int taskId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<WorkTask>> GetRangeByIdsAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    );

    Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetPagedAsync<T>(
        int pageNumber,
        int pageSize,
        Expression<Func<WorkTask, T>> projection,
        TaskFilter? filter = null,
        SortOptions<TaskSortField>? options = null,
        CancellationToken cancellationToken = default
    );

    WorkTask Create(WorkTask task);

    Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    );
}
