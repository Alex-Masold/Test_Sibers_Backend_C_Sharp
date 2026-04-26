using Application.Contracts;
using Application.Contracts.ProjectDocumentContracts;
using Application.Extensions;
using Application.Interfaces;
using Application.Interfaces.Access;
using Domain.Interfaces;
using Domain.Models;
using Domain.Stores;
using FluentValidation;

namespace Application.Services;

public class ProjectDocumentService(
    IProjectDocumentStore documentStore,
    IProjectStore projectStore,
    IFileService fileService,
    IProjectAccessValidator accessValidator,
    IValidator<FileUploadDto> fileValidator,
    IValidator<PagedDto> pagedValidator,
    IUnitOfWork unitOfWork
)
{
    public async Task<ProjectDocumentReadDto> UploadProjectDocumentAsync(
        int projectId,
        FileUploadDto fileDto,
        CancellationToken ct = default
    )
    {
        var project = await projectStore.GetOrThrowAsync(projectId, ct);

        accessValidator.EnsureUpdatePermission(project);

        var validationResult = await fileValidator.ValidateAsync(fileDto, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var storedName = await fileService.SaveFileAsync(fileDto.Content, fileDto.FileName, ct);
        try
        {
            var document = new ProjectDocument
            {
                ProjectId = projectId,
                OriginalFileName = fileDto.FileName,
                StoredFileName = storedName,
                ContentType = fileDto.ContentType,
            };

            var createdDocument = documentStore.Create(document);
            await unitOfWork.SaveChangesAsync(ct);
            return ProjectDocumentReadDto.From(createdDocument);
        }
        catch
        {
            fileService.DeleteFile(storedName);
            throw;
        }
    }

    public async Task<(Stream stream, string contentType, string FileName)> GetDocumentAsync(
        int documentId,
        CancellationToken ct = default
    )
    {
        var document = await documentStore.GetOrThrowAsync(documentId, ct);

        await accessValidator.EnsureReadPermission(document.Project, ct);

        var stream = fileService.GetFileStream(document.StoredFileName);

        return (stream, document.ContentType, document.OriginalFileName);
    }

    public async Task<(
        IReadOnlyCollection<ProjectDocumentReadDto> Items,
        int TotalCount
    )> GetDocumentsAsync(int projectId, PagedDto pagedDto, CancellationToken ct = default)
    {
        var project = await projectStore.GetOrThrowAsync(projectId, ct);

        var validationResult = await pagedValidator.ValidateAsync(pagedDto, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        await accessValidator.EnsureReadPermission(project, ct);

        var result = await documentStore.GetPagedAsync<ProjectDocumentReadDto>(
            project.Id,
            pagedDto.PageNumber,
            pagedDto.PageSize,
            ProjectDocumentReadDto.Projection,
            ct
        );

        return result;
    }

    public async Task DeleteDocumentAsync(int documentId, CancellationToken ct = default)
    {
        var document = await documentStore.GetOrThrowAsync(documentId, ct);
        accessValidator.EnsureUpdatePermission(document.Project);

        await documentStore.DeleteAsync(documentId, ct);
        fileService.DeleteFile(document.StoredFileName);
    }
}
