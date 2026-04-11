using Application.Contracts;
using Application.Contracts.ProjectMemberContracts;
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
            throw new NotFoundException(nameof(ProjectMember), (projectId, employeeId));
        return member;
    }

    private async Task<IReadOnlyCollection<ProjectMember>> GetMembers(
        IReadOnlyCollection<(int ProjectId, int EmployeeId)> pairs,
        CancellationToken ct = default
    )
    {
        var distinctPairs = pairs.Distinct().ToList();
        var existingMembers = await memberStore.GetRangeByIdsAsync(distinctPairs, ct);

        if (existingMembers.Count != distinctPairs.Count)
        {
            var existingId = existingMembers
                .Select(pm => (pm.ProjectId, pm.EmployeeId))
                .ToHashSet();
            var missing = distinctPairs
                .Where(p => !existingId.Contains(p))
                .Select(p => (object)$"({p.ProjectId},{p.EmployeeId})")
                .ToList();
            throw new NotFoundException(nameof(ProjectMember), missing);
        }

        return existingMembers;
    }

    private async Task<bool> EmployeeExists(int employeeId, CancellationToken ct = default)
    {
        var exist = await employeeStore.EmployeeExistsAsync(employeeId, ct);

        if (!exist)
        {
            throw new NotFoundException(nameof(Employee), employeeId);
        }
        return exist;
    }

    private async Task<IReadOnlyCollection<int>> EmployeesExists(
        IReadOnlyCollection<int> employeeIdList,
        CancellationToken ct = default
    )
    {
        var distinctIdList = employeeIdList.Distinct().ToList();
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
        IReadOnlyCollection<int> projectIdList,
        CancellationToken ct = default
    )
    {
        var distinctIdList = projectIdList.Distinct().ToList();
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

        await EmployeeExists(dto.EmployeeId, ct);

        if (await memberStore.MemberExistsAsync(dto.ProjectId, dto.EmployeeId, ct))
        {
            var error = new ValidationFailure(
                propertyName: nameof(ProjectMember.EmployeeId),
                errorMessage: "This employee is already a member of the project"
            );
            throw new ValidationException([error]);
        }

        var entity = dto.ToEntity();

        memberStore.Create(entity);
        await unitOfWork.SaveChangesAsync(ct);

        var createdMember = await GetMember(dto.ProjectId, dto.EmployeeId, ct);

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
        await EmployeesExists(employeeIds, ct);

        foreach (var project in projects)
        {
            accessValidator.EnsureCreatePermission(project);
        }

        var memberIds = dtos.Select(d => (d.ProjectId, d.EmployeeId)).ToList();

        var existing = await memberStore.MembersExistsAsync(memberIds, ct);

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

        var createdIds = entities.Select(e => (e.ProjectId, e.EmployeeId)).ToList();
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

        accessValidator.EnsureDeletePermission(member.Project);

        return await memberStore.DeleteAsync(member.ProjectId, member.EmployeeId, ct);
    }

    public async Task<int> DeleteMembersAsync(
        IReadOnlyCollection<(int ProjectId, int EmployeeId)> pairs,
        CancellationToken ct = default
    )
    {
        var distinctPairs = pairs.Distinct().ToList();

        var members = await GetMembers(distinctPairs, ct);

        var projects = members.Select(m => m.Project).DistinctBy(p => p.Id).ToList();

        foreach (var project in projects)
        {
            accessValidator.EnsureDeletePermission(project);
        }

        return await memberStore.DeleteAsync(distinctPairs, ct);
    }
}
