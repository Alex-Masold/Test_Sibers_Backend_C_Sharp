using System.Linq.Expressions;
using Application.Contracts.Base;
using Domain.Models;

namespace Application.Contracts.ProjectMemberContracts;

public record ProjectMemberReadDto : IReadDto<ProjectMember, ProjectMemberReadDto>
{
    public required int Id { get; init; }

    public int? ProjectId { get; init; }
    public int? EmployeeId { get; init; }

    public string? ProjectName { get; init; }
    public string? EmployeeFirstName { get; init; }
    public string? EmployeeMiddleName { get; init; }
    public string? EmployeeLastName { get; init; }

    public static Expression<Func<ProjectMember, ProjectMemberReadDto>> Projection =>
        member =>
            new()
            {
                Id = member.Id,

                ProjectId = member.ProjectId,
                EmployeeId = member.EmployeeId,

                ProjectName = member.Project.Name,
                EmployeeFirstName = member.Employee.FirstName,
                EmployeeMiddleName = member.Employee.MiddleName,
                EmployeeLastName = member.Employee.LastName,
            };

    public static ProjectMemberReadDto From(ProjectMember member) =>
        new()
        {
            Id = member.Id,

            ProjectId = member.ProjectId,
            EmployeeId = member.EmployeeId,

            ProjectName = member.Project.Name,
            EmployeeFirstName = member.Employee.FirstName,
            EmployeeMiddleName = member.Employee.MiddleName,
            EmployeeLastName = member.Employee.LastName,
        };
}
