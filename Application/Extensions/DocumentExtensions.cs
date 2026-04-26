using Domain.Exceptions;
using Domain.Models;
using Domain.Stores;

namespace Application.Extensions;

public static class DocumentExtensions
{
    public static async Task<ProjectDocument> GetOrThrowAsync(
        this IProjectDocumentStore documentStore,
        int documentId,
        CancellationToken ct = default
    )
    {
        var document = await documentStore.GetByIdAsync(documentId, ct);
        if (document is null)
            throw new NotFoundException(nameof(ProjectDocument), documentId);
        return document;
    }
}
