using Domain.Base;

namespace Domain.Models;

public class Employee : Entity
{
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }

    public Role Role { get; set; } = Role.Worker;

    public IReadOnlyCollection<WorkTask> AssignedTasks { get; init; } = new List<WorkTask>();
    public IReadOnlyCollection<WorkTask> AuthoredTasks { get; init; } = new List<WorkTask>();
    public IReadOnlyCollection<Project> ManagedProjects { get; init; } = new List<Project>();
    public IReadOnlyCollection<ProjectMember> Memberships { get; init; } =
        new List<ProjectMember>();
}
