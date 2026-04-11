using System.Linq.Expressions;
using Domain.Models;
using Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Persistence.DataContext;
using Persistence.Extensions.Helpers;

namespace Persistence.Repositories;

public class ProjectDocumentRepository(ApplicationContext context) : IProjectDocumentStore
{
    /// <summary>
    /// return the tracked object for updating via UnitOfWork.
    /// </summary>
    public async Task<ProjectDocument?> GetByIdAsync(
        int documentId,
        CancellationToken cancellationToken = default
    )
    {
        var document = await context
            .ProjectDocuments.Include(d => d.Project)
            .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

        return document;
    }

    public async Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetPagedAsync<T>(
        int projectId,
        int pageNumber,
        int pageSize,
        Expression<Func<ProjectDocument, T>> projection,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.ProjectDocuments.AsNoTracking().Where(d => d.ProjectId == projectId);

        var result = await query
            .OrderBy(d => d.Id)
            .ToPagedListAsync(pageNumber, pageSize, projection, cancellationToken);

        return result;
    }

    public ProjectDocument Create(ProjectDocument document)
    {
        var createdDocument = context.ProjectDocuments.Add(document);
        return createdDocument.Entity;
    }

    public async Task<int> DeleteAsync(int key, CancellationToken cancellationToken = default)
    {
        return await context
            .ProjectDocuments.Where(e => e.Id == key)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
