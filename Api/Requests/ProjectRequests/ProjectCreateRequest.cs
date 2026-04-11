using Application.Contracts.ProjectContracts;

namespace Api.Requests.ProjectRequests;

public record ProjectCreateRequest
{
    public required string Name { get; init; }

    public required int Priority { get; init; }
    public required string CompanyOrdering { get; init; }
    public string? CompanyExecuting { get; init; }

    public required DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }

    public int? ManagerId { get; init; }

    public ProjectCreateDto ToDto() =>
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
