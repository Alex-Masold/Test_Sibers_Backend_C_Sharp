using Domain.Base;

namespace Domain.Models;

public class WorkTask : Entity
{
    public required string Title { get; set; }

    public int Priority { get; set; }
    public WorkTaskStatus Status { get; set; }

    public string? Comment { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public int? AuthorId { get; set; }
    public Employee? Author { get; init; }
    public int? ExecutorId { get; set; }
    public Employee? Executor { get; init; }
    public required int ProjectId { get; set; }
    public Project Project { get; init; } = null!;
}
