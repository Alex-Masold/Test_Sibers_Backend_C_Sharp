using Application.Contracts.ProjectMemberContracts;

namespace Api.Requests.ProjectMemberRequests;

public record ProjectMemberCreateRequest
{
    public required int ProjectId { get; init; }
    public required int EmployeeId { get; init; }

    public ProjectMemberCreateDto ToDto() =>
        new()
        {
            ProjectId = ProjectId,
            EmployeeId = EmployeeId
        };
}
