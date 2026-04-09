namespace Api.Requests.ProjectMemberRequests;

public record ProjectMemberDeleteRequest
{
    public int ProjectId { get; init; }
    public int EmployeeId { get; init; }

    public (int ProjectId, int EmployeeId) ToEntity() =>
        new() { ProjectId = ProjectId, EmployeeId = EmployeeId };
}
