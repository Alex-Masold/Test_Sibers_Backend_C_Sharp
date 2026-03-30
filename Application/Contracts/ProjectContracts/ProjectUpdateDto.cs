using Application.Contracts.Base;
using Domain.Common;
using Domain.Models;
using Shared.Helpers;

namespace Application.Contracts.ProjectContracts;

public record ProjectUpdateDto : IUpdateDto<Project>
{
    public string? Name { get; init; }
    public int? Priority { get; init; }

    public string? CompanyOrdering { get; init; }
    public Optional<string?> CompanyExecuting { get; init; }

    public DateOnly? StartDate { get; init; }
    public Optional<DateOnly?> EndDate { get; init; }
    public Optional<int?> ManagerId { get; init; }

    public bool ApplyTo(Project project)
    {
        var changed = false;

        if (Name is not null)
        {
            var normalized = Name.Trim();
            if (!string.Equals(project.Name, normalized, StringComparison.Ordinal))
            {
                project.Name = normalized;
                changed = true;
            }
        }

        if (Priority is not null && project.Priority != Priority.Value)
        {
            project.Priority = Priority.Value;
            changed = true;
        }

        if (CompanyOrdering is not null)
        {
            var normalized = CompanyOrdering.Trim();
            if (!string.Equals(project.CompanyOrdering, normalized, StringComparison.Ordinal))
            {
                project.CompanyOrdering = normalized;
                changed = true;
            }
        }

        if (CompanyExecuting.HasValue)
        {
            var normalized = StringHelpers.NormalizeOrNull(CompanyExecuting.Value);
            if (!string.Equals(project.CompanyExecuting, normalized, StringComparison.Ordinal))
            {
                project.CompanyExecuting = normalized;
                changed = true;
            }
        }

        if (StartDate is not null && project.StartDate != StartDate)
        {
            project.StartDate = StartDate.Value;
            changed = true;
        }

        if (EndDate.HasValue && project.EndDate != EndDate.Value)
        {
            project.EndDate = EndDate.Value;
            changed = true;
        }

        if (ManagerId.HasValue && project.ManagerId != ManagerId.Value)
        {
            project.ManagerId = ManagerId.Value;
            changed = true;
        }

        return changed;
    }
}
