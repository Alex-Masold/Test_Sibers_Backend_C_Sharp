using System.Linq.Expressions;
using Domain.Filters;
using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;
using Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Persistence.DataContext;
using Persistence.Extensions.Filters;
using Persistence.Extensions.Helpers;
using Persistence.Extensions.Sort;

namespace Persistence.Repositories;

public class TaskRepository(ApplicationContext context) : ITaskStore
{
    public async Task<WorkTask?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await context
            .Tasks.Include(t => t.Author)
            .Include(t => t.Executor)
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        return task;
    }

    public async Task<IReadOnlyCollection<WorkTask>> GetRangeByIdsAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    )
    {
        var tasks = await context
            .Tasks.Include(t => t.Project)
            .Where(t => idList.Contains(t.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return tasks;
    }

    public async Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetPagedAsync<T>(
        int pageNumber,
        int pageSize,
        Expression<Func<WorkTask, T>> projection,
        TaskFilter? filter = null,
        SortOptions<TaskSortField>? options = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Tasks.AsNoTracking().ApplyFilter(filter);

        var result = await query
            .ApplyOrdering(options)
            .ToPagedListAsync(pageNumber, pageSize, projection, cancellationToken);

        return result;
    }

    public WorkTask Create(WorkTask task)
    {
        var createdTask = context.Tasks.Add(task);
        return createdTask.Entity;
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Tasks.Where(t => t.Id == id).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int> DeleteAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Tasks.Where(t => idList.Contains(t.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
