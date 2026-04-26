using Domain.Exceptions;
using Domain.Models;
using Domain.Stores;

namespace Application.Extensions;

public static class ProjectExtensions
{
    public static async Task<Project> GetOrThrowAsync(
        this IProjectStore projectStore,
        int projectId,
        CancellationToken ct = default
    )
    {
        var project = await projectStore.GetByIdAsync(projectId, ct);
        if (project is null)
            throw new NotFoundException(nameof(Project), projectId);
        return project;
    }

    public static async Task<IReadOnlyCollection<Project>> GetOrThrowAsync(
        this IProjectStore projectStore,
        IReadOnlyCollection<int> projectIdList,
        CancellationToken ct = default
    )
    {
        var distinctIds = projectIdList.Distinct().ToList();
        var existingProjects = await projectStore.GetRangeByIdsAsync(projectIdList, ct);

        if (existingProjects.Count != distinctIds.Count)
        {
            var existingId = existingProjects.Select(p => p.Id).ToList();
            var nonExistingIds = distinctIds.Where(id => !existingId.Contains(id)).ToList();
            throw new NotFoundException(nameof(Project), nonExistingIds);
        }

        return existingProjects;
    }
}
