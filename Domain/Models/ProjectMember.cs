using Domain.Base;

namespace Domain.Models;

public class ProjectMember : Entity
{
    public required int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public required int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
}

