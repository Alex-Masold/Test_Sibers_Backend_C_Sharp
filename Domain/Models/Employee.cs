using Domain.Base;

namespace Domain.Models;

public class Employee : Entity
{
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }

    public Role Role { get; set; } = Role.Worker;

    
    private readonly List<WorkTask> _assignedTasks = [];
    private readonly List<WorkTask> _authoredTasks = [];
    private readonly List<Project> _managedProjects = [];
    private readonly List<ProjectMember> _memberships = [];

    public IReadOnlyCollection<WorkTask> AssignedTasks => _assignedTasks;
    public IReadOnlyCollection<WorkTask> AuthoredTasks => _authoredTasks;
    public IReadOnlyCollection<Project> ManagedProjects => _managedProjects;
    public IReadOnlyCollection<ProjectMember> Memberships => _memberships;
}
