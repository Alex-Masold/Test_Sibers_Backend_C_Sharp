using Application.Contracts.Base;
using Domain.Models;

namespace Application.Contracts.ProjectContracts;

public record ProjectCreateDto : ICreateDto<Project>
{
    public required string Name { get; init; }
    public required int Priority { get; init; }
    public required string CompanyOrdering { get; init; }
    public string? CompanyExecuting { get; init; }

    public DateOnly StartDate { get; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EndDate { get; init; }

    public int? ManagerId { get; init; }

    public Project ToEntity() =>
        new Project()
        {
            Name = Name.Trim(),
            Priority = Priority,

            CompanyOrdering = CompanyOrdering.Trim(),
            CompanyExecuting = CompanyExecuting?.Trim(),

            StartDate = StartDate,
            EndDate = EndDate,

            ManagerId = ManagerId,
        };
}

