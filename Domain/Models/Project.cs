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

    
    private readonly List<WorkTask> _tasks = [];
    private readonly List<ProjectMember> _members = [];
    private readonly List<ProjectDocument> _documents = [];

    public IReadOnlyCollection<WorkTask> Tasks => _tasks;
    public IReadOnlyCollection<ProjectMember> Members => _members;
    public IReadOnlyCollection<ProjectDocument> Documents => _documents;
}
