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
    public async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await context
            .Projects.Include(p => p.Manager)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return project;
    }

    public async Task<IReadOnlyCollection<Project>> GetRangeByIdsAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    )
    {
        var projects = await context
            .Projects.Where(p => idList.Contains(p.Id))
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

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Projects.Where(p => p.Id == id).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int> DeleteAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Projects.Where(p => idList.Contains(p.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
