using System.Linq.Expressions;
using Application.Contracts.Base;
using Domain.Models;

namespace Application.Contracts.ProjectDocumentContracts;

public record ProjectDocumentReadDto : IReadDto<ProjectDocument, ProjectDocumentReadDto>
{
    public int Id { get; init; }
    public required string OriginalFileName { get; init; }
    public required string ContentType { get; init; }
    public int ProjectId { get; init; }

    public static Expression<Func<ProjectDocument, ProjectDocumentReadDto>> Projection =>
        document =>
            new()
            {
                Id = document.Id,
                OriginalFileName = document.OriginalFileName,
                ContentType = document.ContentType,
                ProjectId = document.ProjectId,
            };

    public static ProjectDocumentReadDto From(ProjectDocument document) =>
        new()
        {
            Id = document.Id,
            OriginalFileName = document.OriginalFileName,
            ContentType = document.ContentType,
            ProjectId = document.ProjectId,
        };
}

