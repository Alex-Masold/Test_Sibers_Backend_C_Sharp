using Domain.Models;
using Application.Contracts.Base;

namespace Application.Contracts.ProjectMemberContracts;

public record ProjectMemberCreateDto: ICreateDto<ProjectMember>
{
    public int ProjectId { get; init; }
    public int EmployeeId { get; init; }

    public ProjectMember ToEntity() => new ProjectMember()
    {
        ProjectId = ProjectId,
        EmployeeId = EmployeeId
    };
}