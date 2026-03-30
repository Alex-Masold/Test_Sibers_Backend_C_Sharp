using System.Linq.Expressions;
using Application.Contracts.Base;
using Domain.Models;

namespace Application.Contracts.ProjectContracts;

public record ProjectReadDto : IReadDto<Project, ProjectReadDto>
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required int Priority { get; init; }
    public required string CompanyOrdering { get; init; }
    public required string? CompanyExecuting { get; init; }

    public required DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }

    public int? ManagerId { get; init; }

    public static Expression<Func<Project, ProjectReadDto>> Projection =>
        project => new ProjectReadDto
        {
            Id = project.Id,

            Name = project.Name,
            Priority = project.Priority,

            CompanyOrdering = project.CompanyOrdering,
            CompanyExecuting = project.CompanyExecuting,

            StartDate = project.StartDate,
            EndDate = project.EndDate,

            ManagerId = project.ManagerId,
        };

    public static ProjectReadDto From(Project project) =>
        new()
        {
            Id = project.Id,

            Name = project.Name,
            Priority = project.Priority,

            CompanyOrdering = project.CompanyOrdering,
            CompanyExecuting = project.CompanyExecuting,

            StartDate = project.StartDate,
            EndDate = project.EndDate,

            ManagerId = project.ManagerId,
        };
}
