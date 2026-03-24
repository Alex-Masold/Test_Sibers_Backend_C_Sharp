namespace Domain.Filters;

public record ProjectMemberFilter
{
    public int? ProjectId { get; init; }
    public int? EmployeeId { get; init; }
}
