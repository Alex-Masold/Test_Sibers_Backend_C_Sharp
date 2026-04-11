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

public class ProjectRepository(ApplicationContext context) : IProjectStore
{
    /// <summary>
    /// return the tracked object for updating via UnitOfWork.
    /// </summary>
    public async Task<Project?> GetByIdAsync(int key, CancellationToken cancellationToken = default)
    {
        var project = await context
            .Projects.Include(p => p.Manager)
            .FirstOrDefaultAsync(p => p.Id == key, cancellationToken);

        return project;
    }

    public async Task<IReadOnlyCollection<Project>> GetRangeByIdsAsync(
        IReadOnlyCollection<int> projectIdList,
        CancellationToken cancellationToken = default
    )
    {
        if (projectIdList == null || projectIdList.Count == 0)
            return new List<Project>();

        var projects = await context
            .Projects.Where(p => projectIdList.Contains(p.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return projects;
    }

    public async Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetPagedAsync<T>(
        int pageNumber,
        int pageSize,
        Expression<Func<Project, T>> projection,
        ProjectFilter? filter = null,
        SortOptions<ProjectSortField>? options = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Projects.AsNoTracking().ApplyFilter(filter);

        var result = await query
            .ApplyOrdering(options)
            .ToPagedListAsync(pageNumber, pageSize, projection, cancellationToken);

        return result;
    }

    public Project Create(Project project)
    {
        var createdProject = context.Projects.Add(project);
        return createdProject.Entity;
    }

    public async Task<int> DeleteAsync(int key, CancellationToken cancellationToken = default)
    {
        return await context.Projects.Where(p => p.Id == key).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int> DeleteAsync(
        IReadOnlyCollection<int> keyList,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Projects.Where(p => keyList.Contains(p.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
