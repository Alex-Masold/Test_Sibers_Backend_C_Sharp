using Application.Contracts.Base;
using Domain.Common;
using Domain.Models;
using Shared.Helpers;

namespace Application.Contracts.TaskContracts;

public record TaskUpdateDto : IUpdateDto<WorkTask>
{
    public string? Title { get; init; }
    public int? Priority { get; init; }
    public WorkTaskStatus? Status { get; init; }
    public Optional<string?> Comment { get; init; }
    public Optional<int?> ExecutorId { get; init; }
    public int? ProjectId { get; init; }

    public bool ApplyTo(WorkTask task)
    {
        var changed = false;

        if (Title is not null)
        {
            var normalized = Title.Trim();
            if (!string.Equals(task.Title, normalized, StringComparison.Ordinal))
            {
                task.Title = normalized;
                changed = true;
            }
        }

        if (Priority is not null && task.Priority != Priority.Value)
        {
            task.Priority = Priority.Value;
            changed = true;
        }

        if (Status is not null && task.Status != Status.Value)
        {
            task.Status = Status.Value;
            changed = true;
        }

        if (Comment.HasValue)
        {
            var normalized = StringHelpers.NormalizeOrNull(Comment.Value);
            if (!string.Equals(task.Comment, normalized, StringComparison.Ordinal))
            {
                task.Comment = normalized;
                changed = true;
            }
        }

        if (ExecutorId.HasValue && task.ExecutorId != ExecutorId.Value)
        {
            task.ExecutorId = ExecutorId.Value;
            changed = true;
        }

        if (ProjectId is not null && task.ProjectId != ProjectId)
        {
            task.ProjectId = (int)ProjectId;
            changed = true;
        }

        return changed;
    }
}
