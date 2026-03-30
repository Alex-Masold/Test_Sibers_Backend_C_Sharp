using System.Linq.Expressions;
using Domain.Models;

namespace Domain.Stores;

public interface IProjectDocumentStore
{
    Task<ProjectDocument?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<T> Items, int TotalCount)> GetPagedAsync<T>(
        int projectId,
        int pageNumber,
        int pageSize,
        Expression<Func<ProjectDocument, T>> projection,
        CancellationToken cancellationToken = default
    );
    ProjectDocument Create(ProjectDocument document);
    Task<int> DeleteAsync(int documentId, CancellationToken cancellationToken = default);
}
