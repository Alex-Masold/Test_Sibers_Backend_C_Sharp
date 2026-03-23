using System.Linq.Expressions;
using Domain.Models;

namespace Domain.Stores;

public interface IProjectDocumentStore
{
    Task LoadProjectAsync(ProjectDocument document, CancellationToken cancellationToken);
    Task<ProjectDocument?> GetDocumentByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    );

    Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetDocumentsAsync<T>(
        int projectId,
        int pageNumber,
        int pageSize,
        Expression<Func<ProjectDocument, T>> projection,
        CancellationToken cancellationToken = default
    );
    ProjectDocument CreateDocument(ProjectDocument document);
    void DeleteDocument(ProjectDocument document);
}
