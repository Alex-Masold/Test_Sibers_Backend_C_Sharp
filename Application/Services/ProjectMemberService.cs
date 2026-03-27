using Application.Contracts;
using Application.Contracts.ProjectMemberContracts;
using Application.Interfaces;
using Application.Interfaces.Access;
using Domain.Exceptions;
using Domain.Filters;
using Domain.Interfaces;
using Domain.Models;
using Domain.Stores;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Services;

public class ProjectMemberService(
    IProjectStore projectStore,
    IEmployeeStore employeeStore,
    IProjectMemberStore memberStore,
    ICurrentUserService userService,
    IProjectMemberAccessValidator accessValidator,
    IValidator<PagedDto> pagedValidator,
    IUnitOfWork unitOfWork
)
{
    private async Task<ProjectMember> GetMember(
        int projectId,
        int employeeId,
        CancellationToken ct = default
    )
    {
        var member = await memberStore.GetByIdAsync(projectId, employeeId, ct);
        if (member is null)
            throw new NotFoundException(nameof(ProjectMember), projectId);
        return member;
    }

    private async Task<IReadOnlyCollection<ProjectMember>> GetMembers(
        IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        var distinctIdList = idList.Distinct().ToList();
        var existingMembers = await memberStore.GetRangeByIdsAsync(distinctIdList, ct);

        if (existingMembers.Count != distinctIdList.Count)
        {
            var existingId = existingMembers.Select(m => m.Id).ToList();
            var nonExistingIds = distinctIdList.Where(id => !existingId.Contains(id)).ToList();
            throw new NotFoundException(nameof(ProjectMember), nonExistingIds);
        }

        return existingMembers;
    }

    private async Task<bool> EmployeeExist(int employeeId, CancellationToken ct = default)
    {
        var exist = await employeeStore.EmployeeExistAsync(employeeId, ct);

        if (!exist)
        {
            throw new NotFoundException(nameof(Employee), employeeId);
        }
        return exist;
    }

    private async Task<IReadOnlyCollection<int>> EmployeesExist(
        IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        var distinctIdList = idList.Distinct().ToList();
        var existingEmployeeId = await employeeStore.GetExistingIdsAsync(distinctIdList, ct);

        if (existingEmployeeId.Count != distinctIdList.Count)
        {
            var nonExistingIds = distinctIdList
                .Where(id => !existingEmployeeId.Contains(id))
                .ToList();
            throw new NotFoundException(nameof(Employee), nonExistingIds);
        }

        return existingEmployeeId;
    }

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
        var distinctIdList = idList.Distinct().ToList();
        var existingProjects = await projectStore.GetRangeByIdsAsync(distinctIdList, ct);

        if (existingProjects.Count != distinctIdList.Count)
        {
            var existingId = existingProjects.Select(p => p.Id).ToList();
            var nonExistingIds = distinctIdList.Where(id => !existingId.Contains(id)).ToList();
            throw new NotFoundException(nameof(Project), nonExistingIds);
        }

        return existingProjects;
    }

    public async Task<(
        IReadOnlyCollection<ProjectMemberReadDto> Items,
        int TotalCount
    )> GetMembersAsync(
        PagedDto pagedDto,
        ProjectMemberFilter? filter = null,
        CancellationToken ct = default
    )
    {
        var validationResult = await pagedValidator.ValidateAsync(pagedDto, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var result = await memberStore.GetPagedAsync<ProjectMemberReadDto>(
            pagedDto.PageNumber,
            pagedDto.PageSize,
            ProjectMemberReadDto.Projection,
            filter,
            ct
        );

        return (result.Items, result.TotalCount);
    }

    public async Task<ProjectMemberReadDto> CreateMemberAsync(
        ProjectMemberCreateDto dto,
        CancellationToken ct = default
    )
    {
        var project = await GetProject(dto.ProjectId, ct);

        accessValidator.EnsureCreatePermission(project);

        if (await memberStore.MemberExistAsync(dto.ProjectId, dto.EmployeeId, ct))
        {
            var error = new ValidationFailure(
                propertyName: nameof(ProjectMember.EmployeeId),
                errorMessage: "This employee is already a member of the project"
            );
            throw new ValidationException([error]);
        }

        var entity = dto.ToEntity();

        var createdMember = memberStore.Create(entity);
        await unitOfWork.SaveChangesAsync(ct);

        await memberStore.LoadEmployeeAsync(createdMember, ct);
        await memberStore.LoadProjectAsync(createdMember, ct);

        return ProjectMemberReadDto.From(createdMember);
    }

    public async Task<IReadOnlyCollection<ProjectMemberReadDto>> CreateMembersAsync(
        IReadOnlyCollection<ProjectMemberCreateDto> dtos,
        CancellationToken ct = default
    )
    {
        if (dtos == null || dtos.Count == 0)
            return new List<ProjectMemberReadDto>();

        var projectIds = dtos.Select(d => d.ProjectId).ToList();
        var employeeIds = dtos.Select(d => d.EmployeeId).ToList();

        var projects = await GetProjects(projectIds, ct);
        await EmployeesExist(employeeIds, ct);

        foreach (var project in projects)
        {
            accessValidator.EnsureCreatePermission(project);
        }

        var memberIds = dtos.Select(d => (d.ProjectId, d.EmployeeId)).ToList();

        var existing = await memberStore.MemberExistAsync(memberIds, ct);

        if (existing.Count > 0)
        {
            var errors = existing
                .GroupBy(e => e.ProjectId)
                .Select(g => new ValidationFailure(
                    nameof(ProjectMember.EmployeeId),
                    $"Employees [{string.Join(", ", g.Select(e => e.EmployeeId))}] "
                        + $"already members of project {g.Key}"
                ))
                .ToList();
            throw new ValidationException(errors);
        }

        var entities = dtos.Select(d => d.ToEntity()).ToArray();

        memberStore.CreateRange(entities);
        await unitOfWork.SaveChangesAsync(ct);

        var createdIds = entities.Select(e => e.Id).ToList();
        var createdMembers = await memberStore.GetRangeByIdsAsync(createdIds, ct);

        return createdMembers.Select(ProjectMemberReadDto.From).ToList();
    }

    public async Task<int> DeleteMemberAsync(
        int projectId,
        int employeeId,
        CancellationToken ct = default
    )
    {
        var member = await GetMember(projectId, employeeId, ct);

        var project = await GetProject(projectId, ct);
        await EmployeeExist(employeeId, ct);

        if (userService.IsDirector)
        {
            return await memberStore.DeleteAsync(member.Id, ct);
        }

        accessValidator.EnsureDeletePermission(project);

        return await memberStore.DeleteAsync(member.Id, ct);
    }

    public async Task<int> DeleteMembersByIdsAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        var distinctIdList = idList.Distinct().ToList();

        var members = await GetMembers(distinctIdList, ct);
        var projectIds = members.Select(m => m.ProjectId).Distinct().ToList();
        var employeeIds = members.Select(m => m.EmployeeId).Distinct().ToList();

        var projects = await GetProjects(projectIds, ct);
        await EmployeesExist(employeeIds, ct);

        if (userService.IsDirector)
        {
            return await memberStore.DeleteAsync(distinctIdList, ct);
        }

        foreach (var project in projects)
        {
            accessValidator.EnsureDeletePermission(project);
        }

        return await memberStore.DeleteAsync(distinctIdList, ct);
    }
}
