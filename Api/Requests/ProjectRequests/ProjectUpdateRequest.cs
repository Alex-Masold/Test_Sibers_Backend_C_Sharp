using Application.Contracts.ProjectContracts;
using Domain.Common;

namespace Api.Requests.ProjectRequests;

public record ProjectUpdateRequest
{
    public string? Name { get; init; }

    public int? Priority { get; init; }
    public string? CompanyOrdering { get; init; }
    public Optional<string?> CompanyExecuting { get; init; }

    public DateOnly? StartDate { get; init; }
    public Optional<DateOnly?> EndDate { get; init; }

    public Optional<int?> ManagerId { get; init; }

    public ProjectUpdateDto ToDto() =>
        new()
        {
            Name = Name,
            Priority = Priority,
            CompanyOrdering = CompanyOrdering,
            CompanyExecuting = CompanyExecuting,
            StartDate = StartDate,
            EndDate = EndDate,
            ManagerId = ManagerId,
        };
}
