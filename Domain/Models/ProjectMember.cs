using Domain.Base;

namespace Domain.Models;

public class ProjectMember : Entity
{
    public required int ProjectId { get; set; }
    public required Project Project { get; init; }

    public required int EmployeeId { get; set; }
    public required Employee Employee { get; init; }
}
