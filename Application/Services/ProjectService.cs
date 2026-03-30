using Application.Contracts;
using Application.Contracts.ProjectContracts;
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
    IProjectAccessValidator accessValidator,
    IValidator<ProjectCreateDto> createValidator,
    IValidator<ProjectUpdateDto> updateValidator,
    IValidator<PagedDto> pagedValidator,
    IUnitOfWork unitOfWork
)
{
    private async Task<Project> GetProject(int projectId, CancellationToken ct = default)
    {
        var project = await projectStore.GetByIdAsync(projectId, ct);
        if (project == null)
            throw new NotFoundException(nameof(Project), projectId);
        return project;
    }

    private async Task<IReadOnlyCollection<Project>> GetProjects(
        IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        var existingProjects = await projectStore.GetRangeByIdsAsync(idList, ct);

        if (existingProjects.Count != idList.Count)
        {
            var existingId = existingProjects.Select(p => p.Id).ToList();
            var nonExistingIds = idList.Where(id => !existingId.Contains(id)).ToList();
            throw new NotFoundException(nameof(Project), nonExistingIds);
        }

        return existingProjects;
    }

    public async Task<ProjectReadDto> GetProjectAsync(int projectId, CancellationToken ct = default)
    {
        var project = await GetProject(projectId, ct);

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

        var entity = dto.ToEntity();

        var createdProject = projectStore.Create(entity);
        await unitOfWork.SaveChangesAsync(ct);
        return ProjectReadDto.From(createdProject);
    }

    public async Task<ProjectReadDto> UpdateProjectAsync(
        int projectId,
        ProjectUpdateDto dto,
        CancellationToken ct = default
    )
    {
        var project = await GetProject(projectId, ct);

        accessValidator.EnsureUpdatePermission(project, dto);

        var validationContext = new ValidationContext<ProjectUpdateDto>(dto);
        validationContext.RootContextData["ExistingProject"] = project;

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

    public async Task<int> DeleteProjectAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        accessValidator.EnsureDeletePermission();

        var distinctIdList = idList.Distinct().ToList();
        await GetProjects(distinctIdList, ct);

        return await projectStore.DeleteAsync(distinctIdList, ct);
    }
}
