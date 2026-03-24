using System.Linq.Expressions;
using Application.Contracts.Base;
using Domain.Models;

namespace Application.Contracts.ProjectContracts;

public record ProjectListDto : IReadDto<Project, ProjectListDto>
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public int Priority { get; set; } = 1;
    public required string CompanyOrdering { get; set; } = null!;
    public string? CompanyExecuting { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public required int TaskTotalCount { get; set; }
    public required int MembersTotalCount { get; set; }

    public static Expression<Func<Project, ProjectListDto>> Projection =>
        project =>
            new()
            {
                Id = project.Id,
                Name = project.Name,
                Priority = project.Priority,
                CompanyOrdering = project.CompanyOrdering,
                CompanyExecuting = project.CompanyExecuting,

                StartDate = project.StartDate,
                EndDate = project.EndDate,

                TaskTotalCount = project.Tasks.Count,
                MembersTotalCount = project.Members.Count,
            };

    public static ProjectListDto From(Project project) =>
        new()
        {
            Id = project.Id,
            Name = project.Name,
            Priority = project.Priority,
            CompanyOrdering = project.CompanyOrdering,
            CompanyExecuting = project.CompanyExecuting,

            StartDate = project.StartDate,
            EndDate = project.EndDate,

            TaskTotalCount = project.Tasks.Count,
            MembersTotalCount = project.Members.Count,
        };
}
