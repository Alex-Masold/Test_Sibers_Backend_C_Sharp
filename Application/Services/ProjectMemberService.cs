using Application.Contracts;
using Application.Contracts.ProjectMemberContracts;
using Application.Extensions;
using Application.Interfaces.Access;
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
        var project = await projectStore.GetOrThrowAsync(dto.ProjectId, ct);

        accessValidator.EnsureCreatePermission(project);

        await employeeStore.EnsureExists(dto.EmployeeId, ct);

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

        var createdMember = await memberStore.GetOrThrowAsync(dto.ProjectId, dto.EmployeeId, ct);

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

        var projects = await projectStore.GetOrThrowAsync(projectIds, ct);
        await employeeStore.EnsureAllExist(employeeIds, ct);

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
        var member = await memberStore.GetOrThrowAsync(projectId, employeeId, ct);

        accessValidator.EnsureDeletePermission(member.Project);

        return await memberStore.DeleteAsync(member.ProjectId, member.EmployeeId, ct);
    }

    public async Task<int> DeleteMembersAsync(
        IReadOnlyCollection<(int ProjectId, int EmployeeId)> pairs,
        CancellationToken ct = default
    )
    {
        var distinctPairs = pairs.Distinct().ToList();

        var members = await memberStore.GetOrThrowAsync(distinctPairs, ct);

        var projects = members.Select(m => m.Project).DistinctBy(p => p.Id).ToList();

        foreach (var project in projects)
        {
            accessValidator.EnsureDeletePermission(project);
        }

        return await memberStore.DeleteAsync(distinctPairs, ct);
    }
}
