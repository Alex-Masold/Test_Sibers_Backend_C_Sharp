using Application.Contracts.Base;
using Domain.Models;
using Shared.Helpers;

namespace Application.Contracts.TaskContracts;

public record TaskCreateDto : ICreateDto<WorkTask>
{
    public required string Title { get; init; }

    public int Priority { get; init; }
    public WorkTaskStatus Status { get; init; }
    public string? Comment { get; init; }

    public int? ExecutorId { get; init; }
    public required int ProjectId { get; init; }

    public WorkTask ToEntity() =>
        new()
        {
            Title = Title.Trim(),
            Status = Status,
            Priority = Priority,
            Comment = StringHelpers.NormalizeOrNull(Comment),

            ExecutorId = ExecutorId,
            ProjectId = ProjectId,
        };
}
