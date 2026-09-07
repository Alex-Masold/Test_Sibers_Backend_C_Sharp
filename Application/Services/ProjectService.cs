using Application.Contracts;
using Application.Contracts.ProjectContracts;
using Application.Extensions;
using Application.Interfaces;
using Application.Interfaces.Access;
using Domain.Exceptions;
using Domain.Filters;
using Domain.Interfaces;
using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;
using Domain.Stores;
using FluentValidation;

namespace Application.Services;

public class ProjectService(
    IProjectStore projectStore,
    ICurrentUserService userService,
    TimeProvider timeProvider,
    IProjectAccessValidator accessValidator,
    IValidator<ProjectCreateDto> createValidator,
    IValidator<ProjectUpdateDto> updateValidator,
    IValidator<PagedDto> pagedValidator,
    IUnitOfWork unitOfWork
)
{
    public async Task<ProjectReadDto> GetProjectAsync(int projectId, CancellationToken ct = default)
    {
        var project = await projectStore.GetOrThrowAsync(projectId, ct);

        await accessValidator.EnsureReadPermission(project, ct);

        return ProjectReadDto.From(project);
    }

    public async Task<(IReadOnlyCollection<ProjectListDto> Items, int TotalCount)> GetProjectsAsync(
        PagedDto pagedDto,
        ProjectFilter? filter = null,
        SortOptions<ProjectSortField>? options = null,
        CancellationToken ct = default
    )
    {
        var validationResult = await pagedValidator.ValidateAsync(pagedDto, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        filter ??= new ProjectFilter();
        if (!userService.IsDirector)
        {
            filter = filter with { RelatedEmployeeId = userService.UserId };
        }

        var result = await projectStore.GetPagedAsync<ProjectListDto>(
            pagedDto.PageNumber,
            pagedDto.PageSize,
            ProjectListDto.Projection,
            filter,
            options,
            ct
        );

        return result;
    }

    public async Task<ProjectReadDto> CreateProjectAsync(
        ProjectCreateDto dto,
        CancellationToken ct = default
    )
    {
        accessValidator.EnsureCreatePermission();

        var validationResult = await createValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var project = dto.ToEntity();

        if (project.StartDate == default)
            project.StartDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().DateTime);

        var createdProject = projectStore.Create(project);
        await unitOfWork.SaveChangesAsync(ct);
        return ProjectReadDto.From(createdProject);
    }

    public async Task<ProjectReadDto> UpdateProjectAsync(
        int projectId,
        ProjectUpdateDto dto,
        CancellationToken ct = default
    )
    {
        const string rootContextKey = "ExistingProject";

        var project = await projectStore.GetOrThrowAsync(projectId, ct);

        accessValidator.EnsureUpdatePermission(project, dto);

        var validationContext = new ValidationContext<ProjectUpdateDto>(dto);
        validationContext.RootContextData[rootContextKey] = project;

        var validationResult = await updateValidator.ValidateAsync(validationContext, ct);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (dto.ApplyTo(project))
        {
            await unitOfWork.SaveChangesAsync(ct);
        }

        return ProjectReadDto.From(project);
    }

    public async Task<int> DeleteProjectAsync(int projectId, CancellationToken ct = default)
    {
        accessValidator.EnsureDeletePermission();

        var deleted = await projectStore.DeleteAsync(projectId, ct);

        if (deleted == 0)
            throw new NotFoundException(nameof(Project), projectId);

        return deleted;
    }

    public async Task<int> DeleteProjectsAsync(
        IReadOnlyCollection<int> projectIdList,
        CancellationToken ct = default
    )
    {
        accessValidator.EnsureDeletePermission();

        var distinctIdList = projectIdList.Distinct().ToList();
        await projectStore.GetOrThrowAsync(distinctIdList, ct);

        return await projectStore.DeleteAsync(distinctIdList, ct);
    }
}
