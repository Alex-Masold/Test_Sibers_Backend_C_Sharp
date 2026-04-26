using Domain.Exceptions;
using Domain.Models;
using Domain.Stores;

namespace Application.Extensions;

public static class TaskStoreExtensions
{
    public static async Task<WorkTask> GetOrThrowAsync(
        this ITaskStore taskStore,
        int taskId,
        CancellationToken ct = default
    )
    {
        var task = await taskStore.GetByIdAsync(taskId, ct);
        if (task is null)
            throw new NotFoundException(nameof(WorkTask), taskId);
        return task;
    }

    public static async Task<IReadOnlyCollection<WorkTask>> GetOrThrowAsync(
        this ITaskStore taskStore,
        IReadOnlyCollection<int> taskIdList,
        CancellationToken ct = default
    )
    {
        var distinctIdList = taskIdList.Distinct().ToList();
        var existingTasks = await taskStore.GetRangeByIdsAsync(distinctIdList, ct);

        if (existingTasks.Count != distinctIdList.Count)
        {
            var existingIds = existingTasks.Select(t => t.Id).ToHashSet();
            var nonExistingIds = distinctIdList.Where(id => !existingIds.Contains(id)).ToList();
            throw new NotFoundException(nameof(WorkTask), nonExistingIds);
        }
        return existingTasks;
    }
}
