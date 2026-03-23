using Domain.Base;

namespace Domain.Models;

public class Project : Entity
{
    public required string Name { get; set; }
    public int Priority { get; set; } = 1;
    public required string CompanyOrdering { get; set; }
    public string? CompanyExecuting { get; set; }

    public required DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public int? ManagerId { get; set; }
    public Employee? Manager { get; init; }

    public IReadOnlyCollection<WorkTask> Tasks { get; init; } = new List<WorkTask>();
    public IReadOnlyCollection<ProjectMember> Members { get; init; } = new List<ProjectMember>();
    public IReadOnlyCollection<ProjectDocument> Documents { get; init; } =
        new List<ProjectDocument>();
}
