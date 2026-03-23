using System.ComponentModel.DataAnnotations;
using Application.Contracts.ProjectContracts;

namespace Api.Requests.ProjectRequests;

public record ProjectCreateRequest
{
    public required string Name { get; init; }

    [Range(1, 5, ErrorMessage = "The priority must be between 1 and 5")]
    public required int Priority { get; init; }
    public required string CompanyOrdering { get; init; }
    public string? CompanyExecuting { get; init; }

    public DateOnly? EndDate { get; init; }

    public int? ManagerId { get; init; }

    public ProjectCreateDto ToDto() =>
        new()
        {
            Name = Name,
            Priority = Priority,
            CompanyOrdering = CompanyOrdering,
            CompanyExecuting = CompanyExecuting,
            EndDate = EndDate,
            ManagerId = ManagerId,
        };
}
