using System.Linq.Expressions;
using Domain.Models;
using Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Persistence.DataContext;

namespace Persistence.Repositories;

public class ProjectDocumentRepository(ApplicationContext context) : IProjectDocumentStore
{
    public async Task LoadProjectAsync(
        ProjectDocument document,
        CancellationToken cancellationToken
    )
    {
        await context.Entry(document).Reference(d => d.Project).LoadAsync(cancellationToken);
    }

    public async Task<ProjectDocument?> GetDocumentByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var document = await context.ProjectDocuments.FindAsync([id], cancellationToken);
        return document;
    }

    public async Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetDocumentsAsync<T>(
        int projectId,
        int pageNumber,
        int pageSize,
        Expression<Func<ProjectDocument, T>> projection,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.ProjectDocuments.AsNoTracking().Where(d => d.ProjectId == projectId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(d => d.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(projection)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public ProjectDocument CreateDocument(ProjectDocument document)
    {
        var createdDocument = context.ProjectDocuments.Add(document);
        return createdDocument.Entity;
    }

    public void DeleteDocument(ProjectDocument document)
    {
        context.ProjectDocuments.Remove(document);
    }
}
