using Application.Contracts.Base;
using Domain.Models;

namespace Application.Contracts.TaskContracts;

public record TaskCreateDto : ICreateDto<WorkTask>
{
    public required string Title { get; init; }

    public int Priority { get; init; }
    public WorkTaskStatus Status { get; init; }
    public string? Comment { get; init; }

    public int? ExecutorId { get; init; }
    public required int ProjectId { get; init; }

    private string? Normalize(string? str)
    {
        return string.IsNullOrWhiteSpace(str) ? null : str.Trim();
    }

    public WorkTask ToEntity() =>
        new()
        {
            Title = Title.Trim(),
            Status = Status,
            Priority = Priority,
            Comment = Normalize(Comment),

            ExecutorId = ExecutorId,
            ProjectId = ProjectId,
        };
}
